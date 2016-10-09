//<summary>
//  Title   : Manifest Creator
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2009, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using System.Configuration.Install;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CAS.Lib.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Creates manifest for the product.
  /// </summary>
  internal static class ManifestManagement
  {
    #region internal
    /// <summary>
    /// Product type
    /// </summary>
    internal enum ProductType { AllUsers, SingleUser, ClickOnce }
    /// <summary>
    /// Writes the deploy manifest using data provided by the <paramref name="clickOnceAssembly"/>
    /// and the attributes the assembly is decorated.
    /// </summary>
    /// <param name="clickOnceAssembly">The click once assembly.</param>
    /// <param name="alternativeProductName">Name of the alternative product.
    /// If alternativeProductName is null or string.empty, the assembly name is taken.</param>
    /// <remarks>The type of the deployment is always <see cref="ProductType.ClickOnce"/></remarks>
    internal static void WriteDeployManifest(Assembly clickOnceAssembly, string alternativeProductName)
    {
      AssemblyName name = clickOnceAssembly.GetName();
      if (!string.IsNullOrEmpty(alternativeProductName))
        name.Name = alternativeProductName;
      Uri url = new Uri("http://www.commsvr.eu");
      string publisher = GetPublisherFromAssemblyAttributes(clickOnceAssembly);
      WriteDeployManifest(name, ProductType.ClickOnce, publisher, url, url, FileNames.TargetDir);
    }
    internal static void WriteDeployManifest(InstallContext m_Cnt)
    {
      string productName = m_Cnt.Parameters[InstallContextNames.Productname];
      Version version = new Version(m_Cnt.Parameters[InstallContextNames.Version]);
      string allUsers = m_Cnt.Parameters[InstallContextNames.Allusers];
      AssemblyName an = new AssemblyName(productName)
      {
        Version = version,
        Flags = AssemblyNameFlags.None,
        CultureInfo = System.Globalization.CultureInfo.InvariantCulture
      };
      //it is public token from cas.snk
      an.SetPublicKeyToken(new byte[] { 0x88, 0x32, 0xff, 0x1a, 0x67, 0xea, 0x61, 0xa3 });
      ManifestManagement.ProductType type = String.Compare(allUsers, "1") == 0 ? ManifestManagement.ProductType.AllUsers : ManifestManagement.ProductType.SingleUser;
      string publisher = m_Cnt.Parameters[InstallContextNames.Manufacturer];
      Uri supportUrl = new Uri(m_Cnt.Parameters[InstallContextNames.Arphelplink]);
      string _applicationDataFolder = FileNames.ConstructApplicationDataFolder(publisher, productName);
      if (!(new DirectoryInfo(_applicationDataFolder)).Exists)
        throw new InstallException(_applicationDataFolder + " - the directory for application data does not exists");
      m_SetModifyRights(_applicationDataFolder);
      ManifestManagement.WriteDeployManifest(an, type, publisher, supportUrl, supportUrl, _applicationDataFolder);
    }
    /// <summary>
    /// Reads the deploy manifest using default file name
    /// </summary>
    /// <returns>
    /// Object of <see cref="DeployManifest"/> containing the product manifest
    /// </returns>
    internal static DeployManifest ReadDeployManifest()
    {
      return ReadDeployManifest(FileNames.ManifestFileName);
    }
    /// <summary>
    /// Reads the deploy manifest.
    /// </summary>
    /// <param name="ManifestFileName">Name of the manifest file.</param>
    /// <returns></returns>
    internal static DeployManifest ReadDeployManifest(string ManifestFileName)
    {
      if ((new FileInfo(ManifestFileName)).Exists)
      {
        Manifest mm = ManifestReader.ReadManifest(ManifestFileName, false);
        return mm as DeployManifest;
      }
      else
        return null;
    }
    /// <summary>
    /// Deletes the deploy manifest if exists.
    /// </summary>
    internal static void DeleteDeployManifest()
    {
      try
      {
        if (File.Exists(FileNames.ManifestFileName))
        {
          File.SetAttributes(FileNames.ManifestFileName, 0);
          File.Delete(FileNames.ManifestFileName);
        }
        FileNames.UnloadProductManifest();
      }
      catch { }
    }
    #endregion

    #region private
    private static Action<string> m_SetModifyRights = SetModifyRights;
    internal static void ChangeSetModifyRights(Action<string> newSetModifyRights)
    {
      m_SetModifyRights = newSetModifyRights;
    }
    /// <summary>
    /// Writes the deploy manifest.
    /// </summary>
    /// <param name="name">The name of the deployment.</param>
    /// <param name="type">The type <see cref="ProductType"/> of the deployment.</param>
    /// <param name="publisher">The publisher name - used to create the application data folder.</param>
    /// <param name="supportUrl">The support URL.</param>
    /// <param name="deploymentUrl">The deployment URL.</param>
    /// <param name="appData">The path for the application data folder.</param>
    private static void WriteDeployManifest(AssemblyName name, ProductType type, string publisher, Uri supportUrl, Uri deploymentUrl, string appData)
    {
      if (File.Exists(FileNames.ManifestFileName))
        File.SetAttributes(FileNames.ManifestFileName, 0);
      string hex = BitConverter.ToString(name.GetPublicKeyToken());
      hex = hex.Replace("-", "");
      DeployManifest _deployManifest = new DeployManifest()
      {
        AssemblyIdentity = new AssemblyIdentity(name.FullName, name.Version.ToString())
        {
          PublicKeyToken = hex,
          Type = type.ToString(),
        },
        Product = name.Name,
        Publisher = publisher,
        SupportUrl = supportUrl.ToString(),
        DeploymentUrl = deploymentUrl.ToString(),
      };
      FileReference fr = new FileReference()
      {
        TargetPath = appData,
        IsDataFile = true,
        IsOptional = false,
        Group = "license"
      };
      _deployManifest.FileReferences.Add(fr);
      ManifestWriter.WriteManifest(_deployManifest, FileNames.ManifestFileName);
      File.SetAttributes(FileNames.ManifestFileName, FileAttributes.ReadOnly | FileAttributes.Hidden);
    }

    private static void SetModifyRights(string applicationDataFolder)
    {
      DirectorySecurity ds = Directory.GetAccessControl(applicationDataFolder);
      SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
      FileSystemRights rights = FileSystemRights.Modify | FileSystemRights.CreateFiles;
      FileSystemAccessRule rules = new FileSystemAccessRule(identity, rights, InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
      ds.AddAccessRule(rules);
      Directory.SetAccessControl(applicationDataFolder, ds);
    }
    private static string GetPublisherFromAssemblyAttributes(Assembly enteringAssembly)
    {
      string publisher = InstallContextNames.DefaultPublisherName;
      object[] attributes = enteringAssembly.GetCustomAttributes(false);
      foreach (var item in attributes)
        if (item is AssemblyCompanyAttribute)
          publisher = ((AssemblyCompanyAttribute)item).Company;
      return publisher;
    }
    #endregion
  }
}
