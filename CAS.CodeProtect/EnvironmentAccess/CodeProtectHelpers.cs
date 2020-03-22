//___________________________________________________________________________________
//
//  Copyright (C) 2020, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using System.Deployment.Application;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Xml;

namespace UAOOI.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Cryptography helper providing supporting tools.
  /// </summary>
  public static class CodeProtectHelpers
  {
    #region public

    /// <summary>
    /// Initializes an <see cref="RSACryptoServiceProvider"/> object from the key information provided by the XML string <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The XML string containing key information for new <see cref="RSA"/> instance.</param>
    /// <returns>
    /// A new instance of <see cref="RSACryptoServiceProvider"/>.
    /// </returns>
    /// <exception cref="System.Security.Cryptography.CryptographicException">
    /// The format of the <paramref name="key"/> parameter is not valid.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">The <paramref name="key"/> parameter is null</exception>.
    public static RSACryptoServiceProvider CreateRSA(string key)
    {
      RSACryptoServiceProvider my_KeySsignLic = new RSACryptoServiceProvider
      {
        PersistKeyInCsp = false
      };
      my_KeySsignLic.FromXmlString(key);
      return my_KeySsignLic;
    }

    /// <summary>
    /// Gets the arguments form the deployment URI or from the command line.
    /// </summary>
    /// <returns>System.String[].</returns>
    public static string[] GetArguments()
    {
      try
      {
        if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.ActivationUri != null)
        {
          //ClickOnce deployment
          string query = HttpUtility.UrlDecode(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
          if (!string.IsNullOrEmpty(query) && query.StartsWith("?"))
          {
            string[] arguments = query.Substring(1).Split(' ');
            string[] commandLineArgs = new string[arguments.Length + 1];
            commandLineArgs[0] = Environment.GetCommandLineArgs()[0];
            arguments.CopyTo(commandLineArgs, 1);
            return commandLineArgs;
          }
        }
      }
      catch (DeploymentException) { }
      if (AppDomain.CurrentDomain.SetupInformation != null &&
          AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null &&
          AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
      {
        //MSInstaller
        string[] arguments = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
        string[] commandLineArgs = new string[arguments.Length + 1];
        commandLineArgs[0] = Environment.GetCommandLineArgs()[0];
        arguments.CopyTo(commandLineArgs, 1);
        return commandLineArgs;
      }
      //Command line execution.
      return Environment.GetCommandLineArgs();
    }

    #endregion public

    #region internal

    /// <summary>
    /// Gets the entropy.
    /// </summary>
    /// <returns>Array of bytes</returns>
    internal static byte[] GetEntropy()
    {
      return Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken();
    }

    /// <summary>
    /// Sign an XML file.
    /// </summary>
    /// <param name="document">Document to be signed</param>
    /// <param name="rsa">An instance of the <see cref="RSA"/> that provides a selected RSA algorithm.</param>
    /// <remarks>
    /// This document cannot be verified unless the verifying code has the public key with which it was signed.
    /// </remarks>
    internal static void SignXml(XmlDocument document, RSA rsa)
    {
      // Check arguments.
      if (document == null)
        throw new ArgumentException("SignXml: the document parameter cannot be null");
      if (rsa == null)
        throw new ArgumentException("SignXml: the rsa parameter cannot be null");
      // Create a SignedXml object.
      SignedXml signedXml = new SignedXml(document)
      {
        // Add the key to the SignedXml document.
        SigningKey = rsa
      };
      // Create a reference to be signed.
      Reference reference = new Reference
      {
        Uri = ""
      };
      // Add an enveloped transformation to the reference.
      XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
      reference.AddTransform(env);
      // Add the reference to the SignedXml object.
      signedXml.AddReference(reference);
      // Add public key info
      KeyInfo keyInfo = new KeyInfo();
      keyInfo.AddClause(new RSAKeyValue(rsa));
      signedXml.KeyInfo = keyInfo;
      // Compute the signature.
      signedXml.ComputeSignature();
      // Get the XML representation of the signature and save
      // it to an XmlElement object.
      XmlElement xmlDigitalSignature = signedXml.GetXml();
      // Append the element to the XML document.
      document.DocumentElement.AppendChild(document.ImportNode(xmlDigitalSignature, true));
    }

    /// <summary>
    /// Verify the signature of an XML file against an asymmetric RSA algorithm and return the result.
    /// </summary>
    /// <param name="document">Document to be verified.</param>
    /// <param name="rsa">An instance of the <see cref="RSA"/> that provides a selected RSA algorithm.</param>
    /// <returns>True if signature OK otherwise, false.</returns>
    /// <remarks>There must be only one signature.</remarks>
    internal static bool VerifyXml(XmlDocument document, RSA rsa)
    {
      // Check arguments.
      if (document == null)
        throw new ArgumentException("VerifyXml: the document parameter cannot be null");
      if (rsa == null)
        throw new ArgumentException("VerifyXml: the rsa parameter cannot be null");
      // Create a new SignedXml object and pass it the XML document class.
      SignedXml signedXml = new SignedXml(document);
      // Find the "Signature" node and create a new XmlNodeList object.
      XmlNodeList nodeList = document.GetElementsByTagName("Signature");
      // There must be only one signature. Return false if 0 or more than one signatures was found.
      if ((nodeList.Count <= 0) || (nodeList.Count >= 2))
        return false;
      // Load the first <signature> node.
      signedXml.LoadXml((XmlElement)nodeList[0]);
      // Check the signature and return the result.
      return signedXml.CheckSignature(rsa);
    }

    /// <summary>
    /// Saves the keys in the protected area.
    /// </summary>
    /// <param name="rsa">An instance of <see cref="RSACryptoServiceProvider"/> keeping the keys pair to be protected.</param>
    /// <param name="etp">An entropy to be used to protect the keys.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    internal static void SaveKeysInProtectedArea(RSACryptoServiceProvider rsa, byte[] etp)
    {
      string newKeys = rsa.ToXmlString(true);
      byte[] encKeys = ProtectedData.Protect(UnicodeEncoding.ASCII.GetBytes(newKeys), etp, DataProtectionScope.LocalMachine);
      FileInfo m_info = new FileInfo(FileNames.KeysFilePath());
      using (FileStream kyFile = m_info.Create())
      {
        using (BinaryWriter wrt = new BinaryWriter(kyFile))
        {
          wrt.Write(encKeys);
          wrt.Flush();
          m_info.Attributes = FileAttributes.Hidden | FileAttributes.ReadOnly;
        }
      }
    }

    /// <summary>
    /// Reads keys from protected area and creates an instance of <see cref="RSACryptoServiceProvider"/>.
    /// </summary>
    /// <param name="etp">The entropy to be used to protect the keys.</param>
    /// <returns>
    /// An object <see cref="RSACryptoServiceProvider"/> containing keys if contained
    /// in a protection area, otherwise an exception is thrown.
    /// </returns>
    /// <exception cref="System.IO.FileNotFoundException"> The file cannot be found, such as when the keys file does not exist
    /// The file must already exist.
    /// </exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    internal static RSACryptoServiceProvider ReadKeysFromProtectedArea(byte[] etp)
    {
      string xmlKeys = string.Empty;
      byte[] encKeys;
      using (FileStream kyFile = new FileStream(FileNames.KeysFilePath(), FileMode.Open, FileAccess.Read))
      using (BinaryReader rdr = new BinaryReader(kyFile))
        encKeys = rdr.ReadBytes((int)kyFile.Length);
      xmlKeys = UnicodeEncoding.ASCII.GetString(ProtectedData.Unprotect(encKeys, etp, DataProtectionScope.LocalMachine));
      return CreateRSA(xmlKeys);
    }

    /// <summary>
    /// Tries to read keys from the protected area.
    /// </summary>
    /// <param name="etp">The entropy to be used to protect the keys.</param>
    /// <returns>An object <see cref="RSACryptoServiceProvider"/> containing keys if contained
    /// in a protection area, otherwise null.</returns>
    internal static RSACryptoServiceProvider TryReadKeysFromProtectedArea(byte[] etp)
    {
      if (!File.Exists(FileNames.KeysFilePath()))
        return null;
      return ReadKeysFromProtectedArea(etp);
    }

    #endregion internal
  }
}