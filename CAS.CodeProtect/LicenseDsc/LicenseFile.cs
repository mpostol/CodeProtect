//<summary>
//  Title   : License implementation.
//  System  : Microsoft Visual C# .NET 2005
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//  History :
//    MPostol - 10-11-2006
//
//  Copyright (C)2006, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto:techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using CAS.Lib.CodeProtect.EnvironmentAccess;
using CAS.Lib.CodeProtect.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CAS.Lib.CodeProtect.LicenseDsc
{
  /// <summary>
  /// This is the .LIC file object.  This file contains all the fields
  /// a developer should need to create a license of their choice.  It supports
  /// many standard fields and has the ability to store custom key/value pairs.
  /// This file also contains a Constraints List (System.Collections.Generic.List) 
  /// which is responsible for determine the validity of this license.  The 
  /// constraints are used to determine if the license may be used.
  /// </summary>
  [Serializable]
  public class LicenseFile : License, IConstraintItemProvider
  {

    #region private
    private string version = "2.0";
    // Internal information for tracking
    private string failureMessage = String.Empty;
    private bool isDirty = false;
    private bool isReadOnly = false;
    // Product information
    private int m_volumeConstrain = -1;
    private int m_runTimeConstrain = -1;
    private string m_LicenseUID = string.Empty;
    //private string m_LicModificationUID = string.Empty;
    private string m_InstallationInstanceGUID = string.Empty;
    private string m_LicenseKey = string.Empty;
    private string m_ProcessorId = string.Empty;
    private string m_MotherboardDeviceID = string.Empty;
    private string m_HardwareKey = string.Empty;
    private Product product = new Product();
    private Issuer issuer = new Issuer();
    private Statistics statistics = new Statistics();
    //Users information for this license
    private User user = new User();
    //Usage dates
    private DateTime creationDate = new DateTime();
    private DateTime modificationDate = new DateTime();
    private DateTime firstUseDate = new DateTime();
    private readonly IFormatProvider culture = DateTimeFormatInfo.InvariantInfo;
    private const DateTimeStyles my_DateStyle = DateTimeStyles.AdjustToUniversal;
    //Custom information for the license
    private CustomData customData = new CustomData();
    // Limiting Factors for a license.
    private List<IConstraint> constraints = new List<IConstraint>();
    #region static
    /// <summary>
    /// Loads the specified file to create an LicenseFile using issuer public key to validate it.
    /// </summary>
    /// <param name="licFIleName"> A relative or absolute path for the license file.</param>
    /// <returns>Valid license</returns>
    /// <exception cref="LicenseFileException">LicenseFileException if license file is unavailable or invalid.</exception>
    private static LicenseFile LoadFile(string licFIleName)
    {
      LicenseFile license;
      using (Stream lStreem = new FileStream(licFIleName, FileMode.Open, FileAccess.Read))
        license = LoadFile(lStreem);
      if ((license == null) || !string.IsNullOrEmpty(license.InstallationInstanceGUID))
        throw new LicenseFileException(string.Format("A suitable license file {0} couldn't be found.", licFIleName));
      return license;
    }
    private static LicenseFile LoadFile(Stream licenseStream)
    {
      LicenseFile license;
      using (RSACryptoServiceProvider RSA4Iss = CodeProtectHelpers.CreateRSA(Resources.keys))
        license = LoadFile(licenseStream, RSA4Iss);
      return license;
    }
    private static void UpgradeLicense(LicenseFile newLicense)
    {
      LicenseFile oldLicense = null;
      try
      {
        Debug.Assert(newLicense != null, "LoadFile should not return null");
        if (newLicense == null)
          throw new LicenseFileException("New license not found");
        using (RSACryptoServiceProvider my_RSA = CodeProtectHelpers.ReadKeysFromProtectedArea(CodeProtectHelpers.GetEntropy()))
        {
          try
          {
            using (Stream stream = FileNames.CreateLicenseFileStream(FileMode.Open, FileAccess.Read, FileShare.Read))
              oldLicense = LoadFile(stream, my_RSA);
          }
          catch (Exception ex)
          {
            throw new LicenseFileException("Cannot open old license: ", ex);
          }
          Debug.Assert(oldLicense != null, "LoadFile should not return null");
          if (oldLicense == null)
            throw new LicenseFileException("Old License not found");
          try { oldLicense.UpgardeData(newLicense); }
          catch (LicenseFileException ex)
          {
            throw new LicenseFileException("It is unable to upgarde the license data:", ex);
          }
          RemoveAllLicenses();
          using (Stream stream = FileNames.CreateLicenseFileStream(FileMode.Create, FileAccess.Write, FileShare.None))
            oldLicense.SaveFile(stream, my_RSA);
        }
      }
      catch (Exception)
      {
        throw;
      }
      finally
      {
        if (newLicense != null)
          newLicense.Dispose();
        if (oldLicense != null)
          oldLicense.Dispose();
      }
    }
    #endregion
    #region Utilities
    private void ResetIsDirty()
    {
      this.isDirty = false;
      if (this.CustomData != null)
        this.CustomData.Saved();
      if (this.Issuer != null)
        this.Issuer.Saved();
      if (this.Product != null)
        this.Product.Saved();
      if (this.Statistics != null)
        this.Statistics.Saved();
      if (this.User != null)
        this.User.Saved();
      if (this.Constraints.Count > 0)
      {
        for (int i = 0; i < this.Constraints.Count; i++)
          ((IConstraint)this.Constraints[i]).Saved();
      }
    }
    #endregion
    #region Loading & Parsing
    /// <summary>
    /// Parses the XML content of the license. Used internally when the key is passed
    /// into the constructor to build the license object. It is also used by the
    /// LoadFromXml methods.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="System.Xml.XmlNode">XmlNode</see> representing the license.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the license data is null.
    /// </exception>
    private void parseLicense(XmlNode itemsNode)
    {
      if (itemsNode == null)
        throw new ArgumentNullException("The license data is null.");
      XmlNodeList licenseItems = itemsNode.ChildNodes;
      // Parse the items in the license first...
      parseLicenseItems(itemsNode);
      // Now go through the children for the custom values and user items.
      for (int i = 0; i < licenseItems.Count; i++)
      {
        XmlNode licenseItem = licenseItems[i];
        if (licenseItem.Name == "Product")
          this.product = Product.FromXml(licenseItem);
        else if (licenseItem.Name == "User")
          this.user = User.FromXml(licenseItem);
        else if (licenseItem.Name == "CustomData")
          parseCustomFields(licenseItems[i]);
        else if (licenseItem.Name == "Constraints")
          parseConstraintsFields(licenseItems[i]);
        else if (licenseItem.Name == "Issuer")
          this.issuer = Issuer.FromXml(licenseItem);
        else if (licenseItem.Name == "Statistics")
          this.statistics = Statistics.FromXml(licenseItem);
      }
    }
    /// <summary>
    /// Parse the XML content of the license that is not part of another child object.
    /// </summary>
    /// <param name="itemNode">
    /// A <see cref="System.Xml.XmlNode">XmlNode</see> representing the license.
    /// </param>
    private void parseLicenseItems(XmlNode itemNode)
    {
      XmlNode licUIDKeyTextNode = itemNode.SelectSingleNode("LicenseUID/text()");		// String
      XmlNode modKeyTextNode = itemNode.SelectSingleNode("LicModificationUID/text()");		// String
      XmlNode hashKeyTextNode = itemNode.SelectSingleNode("InstallationInstanceGUID/text()");		// String
      XmlNode licenseKeyTextNode = itemNode.SelectSingleNode("LicenseKey/text()");		// String
      XmlNode procKeyTextNode = itemNode.SelectSingleNode("ProcessorId/text()");		// String
      XmlNode mtrbKeyTextNode = itemNode.SelectSingleNode("MotherboardDeviceID/text()");		// String
      XmlNode hardwareKeyTextNode = itemNode.SelectSingleNode("HardwareKey/text()");		// String
      XmlNode creationDateTextNode = itemNode.SelectSingleNode("CreationDate/text()");		// Date
      XmlNode firstUseDateTextNode = itemNode.SelectSingleNode("FirstUseDate/text()");		// Date
      XmlNode modificationDateTextNode = itemNode.SelectSingleNode("ModificationDate/text()");	// Date
      if (licUIDKeyTextNode != null)
        this.m_LicenseUID = licUIDKeyTextNode.Value;
      if (modKeyTextNode != null)
        this.LicModificationUID = modKeyTextNode.Value;
      if (hashKeyTextNode != null)
        this.m_InstallationInstanceGUID = hashKeyTextNode.Value;
      if (licenseKeyTextNode != null)
        this.m_LicenseKey = licenseKeyTextNode.Value;
      if (procKeyTextNode != null)
        this.m_ProcessorId = procKeyTextNode.Value;
      if (mtrbKeyTextNode != null)
        this.m_MotherboardDeviceID = mtrbKeyTextNode.Value;
      if (hardwareKeyTextNode != null)
        this.m_HardwareKey = hardwareKeyTextNode.Value;
      if (creationDateTextNode != null)
        this.creationDate = DateTime.Parse(creationDateTextNode.Value, culture, DateTimeStyles.AdjustToUniversal);
      if (firstUseDateTextNode != null)
        this.firstUseDate = DateTime.Parse(firstUseDateTextNode.Value, culture, DateTimeStyles.AdjustToUniversal);
      if (modificationDateTextNode != null)
        this.modificationDate = DateTime.Parse(modificationDateTextNode.Value, culture, DateTimeStyles.AdjustToUniversal);
    }
    /// <summary>
    /// Parse the XML content of the custom fields section of the license.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="System.Xml.XmlNode">XmlNode</see> representing the Custom Fields
    /// section of the license.
    /// </param>
    private void parseCustomFields(XmlNode itemsNode)
    {
      // Check if custom fields are defined
      if (itemsNode == null)
        return;
      // If they are then process all of them
      XmlNodeList customItems = itemsNode.ChildNodes;
      for (int i = 0; i < customItems.Count; i++)
      {
        XmlNode customItem = customItems[i];
        XmlNode keyTextNode = customItem.SelectSingleNode("Key/text()");		// String
        XmlNode valueTextNode = customItem.SelectSingleNode("Value/text()");	// String
        if (keyTextNode != null && keyTextNode.Value != String.Empty)
          this.customData.AddCustomData(keyTextNode.Value, valueTextNode.Value);
      }
    }
    /// <summary>
    /// Parse the XML content of the constraints group/fields section of the license.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="System.Xml.XmlNode">XmlNode</see> representing the
    /// Constraints List (System.Collections.Generic.List) section of the
    /// license.
    /// </param>
    private void parseConstraintsFields(XmlNode itemsNode)
    {
      // Check if custom fields are defined
      if (itemsNode == null)
        return;
      // If they are then process all of them
      XmlNodeList constraints = itemsNode.ChildNodes;
      for (int i = 0; i < constraints.Count; i++)
      {
        XmlNode constraint = constraints[i];
        XmlNode typeTextNode = constraint.SelectSingleNode("Type/text()");
        if (typeTextNode != null)
        {
          Type constraintType = Type.GetType((String)typeTextNode.Value, false, true);
          ConstructorInfo cInfo = constraintType.GetConstructor(new Type[] { typeof(LicenseFile) });
          IConstraint c = (IConstraint)cInfo.Invoke(new Object[] { this });
          c.FromXml(constraint);
          this.constraints.Add(c);
        }
      }
    }
    #endregion Loading & Parsing
    /// <summary>
    /// Saves the license file.
    /// </summary>
    /// <param name="fileName">Path and name of the file.</param>
    /// <param name="key">The key to be used to signe the license file.</param>
    private void SaveFile(string fileName, RSA key)
    {
      using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
        SaveFile(fs, key);
    }
    /// <summary>
    /// Converts this <see cref="LicenseFile"/> to a <see cref="XmlDocument"/> and adds digital signature.
    /// </summary>
    /// <param name="key"><see cref="RSA"/> key to sign the document</param>
    /// <returns>Signed document.</returns>
    private XmlDocument ToXmlSignedXmlDocument(RSA key)
    {
      XmlDocument outDoc = ToXmlSignedXmlDocument();
      CodeProtectHelpers.SignXml(outDoc, key);
      return outDoc;
    }
    /// <summary>
    /// The constructor for an <c>LicenseFile</c>.
    /// </summary>
    /// <param name="XmlData">A <see cref="System.String">String</see> representing the Xml data of the license.</param>
    private LicenseFile(XmlDocument XmlData)
    {
      parseLicense(XmlData.SelectSingleNode("/License"));
    }
    /// <summary>
    /// Upgrade this object
    /// </summary>
    /// <param name="upgrade">Object to be used to upgrade this object</param>
    /// <returns>false if upgrade is impossible due to inconsistency</returns>
    private void UpgardeData(LicenseFile upgrade)
    {
      if (upgrade.LicenseUID == string.Empty)
        throw new LicenseFileException(Resources.ErrStr_UpgardeData_LicenseUID);
      if ((upgrade.m_HardwareKey != string.Empty) && (upgrade.m_HardwareKey.CompareTo(this.HardwareKeyTokenBasedOnDevice) != 0))
        throw new LicenseFileException(Resources.ErrStr_UpgardeData_HardwareKeyToken);
      if ((upgrade.m_LicenseKey != string.Empty) && (upgrade.m_LicenseKey.CompareTo(this.m_LicenseKey) != 0))
        throw new LicenseFileException(Resources.ErrStr_UpgardeData_LicenseKeyToken);
      if (upgrade.LicenseUID.CompareTo(this.LicModificationUID) == 0 ||
        upgrade.LicenseUID.CompareTo(this.LicenseUID) == 0)
        throw new LicenseFileException(Resources.ErrStr_UpgardeData_Overupgrade);
      //Shallow conditions are met, start modification process with deep check. 
      this.LicModificationUID = this.LicenseUID; // in the LicModificationUID the previous LicenseUID is stored
      this.m_LicenseUID = upgrade.LicenseUID; // LicenseUID of the new license is now the current ID
      try
      {
        this.Product.UpgardeData(upgrade.Product);
      }
      catch (Exception ex)
      {
        throw new LicenseFileException(Resources.ErrStr_UpgardeData_Product, ex);
      }
      if (!this.Issuer.UpgardeData(upgrade.Issuer))
        throw new LicenseFileException(Resources.ErrStr_UpgardeData_Issuer);
      if (!this.User.UpgardeData(upgrade.User))
        throw new LicenseFileException(Resources.ErrStr_UpgardeData_User);
      if (upgrade.m_HardwareKey != string.Empty)
        this.m_HardwareKey = upgrade.m_HardwareKey;
      this.Constraints = upgrade.Constraints;
      this.ModificationDate = DateTime.Now;
      this.InstallationInstanceGUID = Guid.NewGuid().ToString();
      IsDirty = true;
    }
    /// <summary>
    /// Removes all license files from application dir and isolated storage
    /// </summary>
    private static void RemoveAllLicenses()
    {
      try
      {
        File.Delete(FileNames.LicenseFilePath);
      }
      catch { }
      try
      {
        IsolatedStorageFile isoFile = IsolatedStorageFile.GetStore
          (IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        isoFile.Remove();
      }
      catch { }
    }
    private void InitializeLicense(string ca_user, string ca_companay, string ca_email)
    {
      InitializeLicense();
      Product.SetProduct(FileNames.ProductManifest());
      User.Name = ca_user;
      User.Organization = ca_companay;
      User.Email = ca_email;
    }
    private void InitializeLicense(LicenseFile license)
    {
      InitializeLicense();
      Product = license.Product;
      User = license.User;
    }
    private void InitializeLicense()
    {
      this.InstallationInstanceGUID = Guid.NewGuid().ToString();
      LicenseKeyToken = LicenseKey;
      SetProcessorId = WMIAccess.GetProcessorId;
      SetMotherboardDeviceID = WMIAccess.GetMotherboardDeviceID;
      FirstUsesDate = DateTime.Now;
      Product.FilePath = FileNames.TargetDir;
    }
    #endregion

    #region creators
    /// <summary>
    /// Creates an empty license
    /// </summary>
    public LicenseFile() { }
    #endregion creators

    #region Static
    /// <summary>
    /// Loads the specified file to create an LicenseFile
    /// </summary>
    /// <param name="stream">The stream to use to open the file.</param>
    /// <param name="key">The encryption key to be used to digitally sign the file.</param>
    /// <param name="checkSignature">if true checks signature using given key</param>
    /// <returns>An instance of <see cref="LicenseFile"/> if found, otherwise null.</returns>
    public static LicenseFile LoadFile(Stream stream, RSACryptoServiceProvider key, bool checkSignature)
    {
      if (stream == null)
        throw new ArgumentNullException("stream cannot be null");
      if ((key == null) && checkSignature)
        throw new ArgumentNullException("key cannot be null");
      XmlDocument xmlData = new XmlDocument();
      string xml;
      using (StreamReader streamReader = new StreamReader(stream))
        xml = streamReader.ReadToEnd();
      xmlData.LoadXml(xml);
      if (checkSignature && !CodeProtectHelpers.VerifyXml(xmlData, key))
        throw new LicenseFileException(Resources.ErrStr_OpenLicense_Signature);
      LicenseFile license = new LicenseFile(xmlData);
      license.ResetIsDirty();
      return license;
    }
    /// <summary>
    /// Loads the specified file to create an LicenseFile and checks the digital signature
    /// </summary>
    /// <param name="stream">
    /// The stream to use to open the file.
    /// </param>
    /// <param name="key">
    /// The encryption key to be used to digitally sign the file.
    /// </param>
    /// <returns>new license object or null if stream == null </returns>
    public static LicenseFile LoadFile(Stream stream, RSACryptoServiceProvider key)
    {
      return LoadFile(stream, key, true);
    }
    /// <summary>
    /// Loads the specified file to create a LicenseFile.
    /// </summary>
    /// <param name="license">An instance of the <see cref="FileInfo"/> containing all information about the license file.</param>
    /// <param name="key">The products public keys to verify the license signature..</param>
    /// <param name="checkSignature">if set to <c>true</c> the signature is checked.</param>
    /// <returns></returns>
    public static LicenseFile LoadFile(FileInfo license, RSACryptoServiceProvider key, bool checkSignature)
    {
      FileStream fileStream = null;
      fileStream = license.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
      return LoadFile(fileStream, key, checkSignature);
    }
    /// <summary>
    /// Upgrades the license.
    /// </summary>
    /// <param name="license">The license to be used.</param>
    public static void UpgradeLicense(Stream license)
    {
      try
      {
        LicenseFile newLicense = LoadFile(license);
        UpgradeLicense(newLicense);
      }
      catch (Exception ex)
      {
        throw new LicenseFileException(Resources.ErrorStringUnableToUpgradeLicense, ex);
      }
    }
    /// <summary>
    /// Upgrades the license
    /// </summary>
    /// <param name="newLicFName">A relative or absolute path for the new license file.</param>
    /// <exception cref="LicenseFileException">
    /// The <see cref="LicenseFileException"/> object with a specified error message and 
    /// a reference to the inner exception that is the cause of this exception.
    /// </exception>
    public static void UpgradeLicense(string newLicFName)
    {
      FileInfo licInfo = new FileInfo(newLicFName);
      if (!licInfo.Exists)
        throw new LicenseFileException("New license not found");
      try
      {
        LicenseFile newLicense = null;
        try
        {
          newLicense = LoadFile(licInfo.FullName);
        }
        catch (LicenseFileException ex)
        {
          throw new LicenseFileException("Cannot open new license: ", ex);
        }
        UpgradeLicense(newLicense);
      }
      catch (Exception ex)
      {
        throw new LicenseFileException(Resources.ErrorStringUnableToUpgradeLicense, ex);
      }
    }
    /// <summary>
    /// Installs the specified license that is loaded using issuer public keys embedded in the resource. 
    /// </summary>
    /// <param name="license">The license that is to be installed. It must be signed by the issuer private key.</param>
    /// <exception cref="LicenseFileException">Cannot install license from the provided stream.</exception>
    internal static void Install(Stream license)
    {
      LicenseFile oldLicense = null;
      try
      {
        using (RSACryptoServiceProvider rsa = CodeProtectHelpers.ReadKeysFromProtectedArea(CodeProtectHelpers.GetEntropy()))
        {
          using (Stream stream = FileNames.CreateLicenseFileStream(FileMode.Open, FileAccess.Read, FileShare.Read))
            oldLicense = LicenseFile.LoadFile(stream, rsa, true);
          //Load license using issuer public keys
          using (LicenseFile _newLicense = LicenseFile.LoadFile(license))
          {
            _newLicense.InitializeLicense(oldLicense);
            RemoveAllLicenses();
            using (Stream stream = FileNames.CreateLicenseFileStream(FileMode.Create, FileAccess.Write, FileShare.None))
              _newLicense.SaveFile(stream, rsa);
          }
        }
      }
      catch (Exception ex)
      {
        throw new LicenseFileException("Cannot install license from the stream", ex);
      }
      finally
      {
        if (oldLicense != null)
          oldLicense.Dispose();
      }
    }
    /// <summary>
    /// Performs the installation. Reads license using issuer public key and after instance modification writes
    /// it back using new generated private key.
    /// </summary>
    /// <param name="ca_user">User information for the new license file.</param>
    /// <param name="ca_companay">Company information for the new license file.</param>
    /// <param name="ca_email">Email information for the new license file.</param>
    /// <param name="licenseFileName">Full path name of the license file.</param>
    /// <param name="LoadLicenseFromDefaultContainer">if set to <c>true</c> license is loaded from default container.</param>
    /// <param name="LicenseUnlockCode">The license unlock code.</param>
    internal static void Install(string ca_user, string ca_companay, string ca_email, string licenseFileName, bool LoadLicenseFromDefaultContainer, string LicenseUnlockCode)
    {
      //Open old keys pair if exist ( useful in debug mode )
      RSACryptoServiceProvider rsa = CodeProtectHelpers.TryReadKeysFromProtectedArea(CodeProtectHelpers.GetEntropy());
      try
      {
        if (rsa == null)
        {
          //Create new keys in case the old do not exist.
          rsa = new RSACryptoServiceProvider();
          CodeProtectHelpers.SaveKeysInProtectedArea(rsa, CodeProtectHelpers.GetEntropy());
        }
        else
        {
          if (!LoadLicenseFromDefaultContainer)
          {
            //try to open license using old keys - useful in the debug mode.
            using (Stream str = FileNames.CreateLicenseFileStream(FileMode.Open, FileAccess.Read, FileShare.None))
            {
              try
              {
                LicenseFile lic = LicenseFile.LoadFile(str, rsa);
                if (lic != null)
                  return;
              }
              catch { }
            }
          }
        }
        //Load license using issuer public keys
        LicenseFile my_lic = null;
        if (LoadLicenseFromDefaultContainer)
        {
          UnlockKeyAssemblyContainer ukac = new UnlockKeyAssemblyContainer();
          if (string.IsNullOrEmpty(LicenseUnlockCode))
            LicenseUnlockCode = Resources.DefaultUnlockCode;
          string path = ukac.GetManifestResourcePath(LicenseUnlockCode);
          my_lic = LicenseFile.LoadFile(ukac.GetManifestResourceStream(path));
        }
        else
        {
          my_lic = LicenseFile.LoadFile(licenseFileName);
        }
        my_lic.InitializeLicense(ca_user, ca_companay, ca_email);
        my_lic.SaveFile(licenseFileName, rsa);
      }
      finally
      {
        if (rsa != null)
          rsa.Clear();
      }
    }
    internal static void Uninstal()
    {
      RemoveAllLicenses();

    }
    #endregion

    #region public
    #region Save
    /// <summary>
    /// Saves the license to the specified path using the provided <see cref="Stream"/> by the 
    /// 	<paramref name="stream"/> parameter.
    /// </summary>
    /// <param name="stream">The stream to write the file to.</param>
    /// <param name="key">The key to be used to signe the license file.</param>
    public void SaveFile(Stream stream, RSA key)
    {
      this.ModificationDate = DateTime.Now;
      ToXmlSignedXmlDocument(key).Save(stream);
      this.ResetIsDirty();
    }
    #endregion
    /// <summary>
    /// Resets the date information of this license.  This includes the Creation,
    /// Modification, and First Use dates.  It also includes changing the Statistics
    /// DateTimeLastAccessed since this is for User Access info not creator.
    /// </summary>
    public void ResetCreationStatistics()
    {
      this.m_LicenseUID = Guid.NewGuid().ToString();
      this.CreationDate = DateTime.Now;
      this.modificationDate = this.CreationDate;
      this.firstUseDate = new DateTime(0);
      this.Statistics.ResetUserStatistics();
    }
    /// <summary>
    /// Returns a <see cref="System.String">String</see> that represents the current <c>LicenseFile</c>.
    /// This currently just returns the product full name.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String">String</see> representing this <c>LicenseFile</c>.
    /// </returns>
    public override string ToString()
    {
      return this.product.FullName;
    }
    /// <summary>
    /// Converts this <see cref="LicenseFile"/> to a <see cref="XmlDocument"/> containing the license.
    /// </summary>
    /// <returns>
    /// A <see cref="XmlDocument"/> representing the this <see cref="LicenseFile"/>.
    /// </returns>
    public XmlDocument ToXmlSignedXmlDocument()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(xmlString));
      xmlWriter.Formatting = Formatting.Indented;
      xmlWriter.IndentChar = '\t';
      xmlWriter.WriteStartDocument();
      xmlWriter.WriteComment("Do not touch this file !!! It is digitally signet license.");
      xmlWriter.WriteComment("Any modification will cause lose of integrity and make it invalid.");
      xmlWriter.WriteStartElement("License");
      xmlWriter.WriteAttributeString("Version", "", this.version);
      xmlWriter.WriteElementString("LicenseUID", m_LicenseUID);
      xmlWriter.WriteElementString("LicModificationUID", LicModificationUID);
      xmlWriter.WriteElementString("InstallationInstanceGUID", m_InstallationInstanceGUID);
      xmlWriter.WriteElementString("LicenseKey", m_LicenseKey);
      xmlWriter.WriteElementString("ProcessorId", m_ProcessorId);
      xmlWriter.WriteElementString("MotherboardDeviceID", m_MotherboardDeviceID);
      xmlWriter.WriteElementString("HardwareKey", m_HardwareKey);
      xmlWriter.WriteElementString("CreationDate", this.CreationDate.ToString(culture));
      xmlWriter.WriteElementString("FirstUseDate", this.FirstUsesDate.ToString(culture));
      xmlWriter.WriteElementString("ModificationDate", this.ModificationDate.ToString(culture));
      xmlWriter.WriteRaw(this.Statistics.ToXmlString());
      xmlWriter.WriteRaw(this.Issuer.ToXmlString());
      xmlWriter.WriteRaw(this.Product.ToXmlString());
      if (constraints != null && constraints.Count > 0)
      {
        xmlWriter.WriteStartElement("Constraints");
        for (int i = 0; i < constraints.Count; i++)
          xmlWriter.WriteRaw(((IConstraint)constraints[i]).ToXmlString());
        xmlWriter.WriteEndElement(); // constraints
      }
      if (this.CustomData != null && this.CustomData.Items != null && this.CustomData.Items.Count > 0)
        xmlWriter.WriteRaw(this.CustomData.ToXmlString());
      xmlWriter.WriteRaw(user.ToXmlString());
      xmlWriter.WriteEndElement(); // license
      xmlWriter.WriteEndDocument();
      xmlWriter.Close();
      XmlDocument outDoc = new XmlDocument();
      outDoc.LoadXml(xmlString.ToString());
      return outDoc;
    }
    /// <summary>
    /// Validate this license against the defined <see cref="IConstraint">IConstraints</see> in the
    /// Constraints List (System.Collections.Generic.List).
    /// </summary>
    /// <param name="typeToValidate">Validates this <see cref="LicenseFile"/> for <see cref="Type"/> provided by the <paramref name="typeToValidate"/>.
    /// </param>
    /// <returns>
    /// 	<c>True</c> if the license is valid; Otherwise <c>False</c>.
    /// </returns>
    public bool ValidateLicense(Type typeToValidate)
    {
      if (typeToValidate == null)
        throw new ArgumentNullException("typeToValidate");
      //If the license itself is licensed then don't bother checking the constraints
      if (Product.IsLicensed)
        return true;
      Warnings = new List<string>();
      for (int i = 0; i < constraints.Count; i++)
      {
        m_volumeConstrain = -1;
        m_runTimeConstrain = -1;
        if (constraints[i].Validate(typeToValidate, ref m_volumeConstrain, ref m_runTimeConstrain))
        {
          this.FailureReason = String.Empty;
          return true;
        }
      }
      if (String.IsNullOrEmpty(this.FailureReason))
        this.FailureReason = String.Format("No valid constraint found for the type: Name={0}, Guid={1}", typeToValidate.Name, typeToValidate.GUID.ToString());
      return false;
    }
    /// <summary>
    /// Gets or sets the wornings.
    /// </summary>
    /// <value>The wornings.</value>
    [System.ComponentModel.Browsable(false)]
    public List<string> Warnings { get; private set; }
    #endregion

    #region Internal
    internal string SetProcessorId { set { m_ProcessorId = value; } }
    internal string SetMotherboardDeviceID { set { m_MotherboardDeviceID = value; } }
    internal void AddWorning(string worning)
    {
      Warnings.Add(worning);
    }
    #endregion internal

    #region Public Properties
    /// <summary>
    /// The license unique identifier
    /// </summary>
    /// <returns>
    ///	Gets the license unique identifier
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("The license unique identifier"),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(true)
    ]
    public string LicenseUID
    {
      get { return m_LicenseUID; }
    }
    /// <summary>
    /// The license unique identifier used to upgrade this license.
    /// </summary>
    /// <returns>
    ///	Gets the license unique identifier used to upgrade this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("The license unique identifier used to upgrade this license"),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(true)
    ]
    public string LicModificationUID { get; private set; }
    /// <summary>
    /// Gets the license key (XML string representing the license) granted for this license.
    /// </summary>
    /// <returns>
    ///	Gets the license key (XML string representing the license) granted for this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Gets the license encYey (XML string representing the license) granted for this license."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(true)
    ]
    public string InstallationInstanceGUID
    {
      get { return m_InstallationInstanceGUID; }
      set { m_InstallationInstanceGUID = value; }
    }
    /// <summary>
    /// Gets or Sets the reason a failure occurred. This is an empty
    /// <see cref="System.String">String</see> if no error occurred.
    /// </summary>
    /// <param>
    /// Sets the reason a failure occurred.  Should never be directly set by a user.
    /// </param>
    /// <returns>
    ///	A <see cref="System.String">String</see> representing the reason for the
    /// failure to obtain a license.  An empty <see cref="System.String">String</see>
    /// if no failure occurred.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Gets or Sets the reason a failure occurred. This is an empty String if no error occurred."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)
    ]
    public String FailureReason
    {
      get
      {
        return this.failureMessage;
      }
      internal set
      {
        this.failureMessage = value;
      }
    }
    /// <summary>
    /// Gets if this license was modified since it was last saved.
    /// </summary>
    /// <returns>
    ///	Gets if this license was modified since it was last saved.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Gets if this license was modified since it was last saved."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)
    ]
    public bool IsDirty
    {
      get
      {
        if (this.isDirty)
          return true;
        if (this.Statistics != null && this.Statistics.IsDirty)
          return true;
        if (this.Product != null && this.Product.IsDirty)
          return true;
        if (this.Issuer != null && this.Issuer.IsDirty)
          return true;
        if (this.User != null && this.User.IsDirty)
          return true;
        if (this.CustomData != null && this.CustomData.IsDirty)
          return true;
        for (int i = 0; i < this.Constraints.Count; i++)
        {
          if (((IConstraint)this.Constraints[i]).IsDirty)
            return true;
        }
        return false;
      }
      set { this.isDirty = true; }
    }
    /// <summary>
    /// Gets or Sets if this license can only be read and not modified.
    /// </summary>
    /// <param>
    /// Sets if this license can only be read and not modified.
    /// </param>
    /// <returns>
    ///	Gets if this license can only be read and not modified.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Gets or Sets if this license can only be read and not modified."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)
    ]
    public bool IsReadOnly
    {
      get
      {
        return this.isReadOnly;
      }
      set
      {
        this.isReadOnly = value;
        this.isDirty = true;
      }
    }
    ///// <summary>
    ///// Gets object Type this license is assigned to.
    ///// </summary>
    ///// <returns>
    /////	Gets object Type this license is assigned to.
    ///// </returns>
    //[
    //Bindable(false),
    //Browsable(false),
    //Category("Data"),
    //System.ComponentModel.DefaultValue(null),
    //System.ComponentModel.Description("Gets object Type this license is assigned to."),
    //DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
    //]
    //public Type Type
    //{
    //  get
    //  {
    //    return this.my_Type;
    //  }
    //}
    /// <summary>
    /// Gets or Sets the <see cref="Product"/> object declared for this license.
    /// </summary>
    /// <param>
    ///	Sets the <see cref="Product"/> object declared for this license.
    /// </param>
    /// <returns>
    ///	Gets the <see cref="Product"/> object declared for this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(null),
    System.ComponentModel.Description("Gets or Sets the Product object declared for this license."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public Product Product
    {
      get
      {
        return this.product;
      }
      set
      {
        this.product = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// Gets or Sets the <see cref="User"/> object who owns this license.
    /// </summary>
    /// <param>
    /// Sets the <see cref="User"/> object who owns this license.
    /// </param>
    /// <returns>
    ///	Gets the <see cref="User"/> object who owns this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(null),
    System.ComponentModel.Description("Gets or Sets the User object who owns this license."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public User User
    {
      get
      {
        return this.user;
      }
      set
      {
        this.user = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// Gets or Sets the <see cref="Statistics"/> object for this license.
    /// </summary>
    /// <param>
    /// Sets the <see cref="Statistics"/> object for this license.
    /// </param>
    /// <returns>
    ///	Gets the <see cref="Statistics"/> object for this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(null),
    System.ComponentModel.Description("Gets or Sets the Statistics object for this license."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public Statistics Statistics
    {
      get
      {
        return this.statistics;
      }
      set
      {
        this.statistics = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// Gets the DateTime this license was created.
    /// </summary>
    /// <returns>
    ///	Gets the DateTime this license was created.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(null),
    System.ComponentModel.Description("Gets the DateTime this license was created."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public DateTime CreationDate
    {
      get
      {
        if (this.creationDate.Ticks > 0)
          return this.creationDate;
        else
          return DateTime.Now;
      }
      set
      {
        this.creationDate = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// Gets the DateTime this license was last modified.
    /// </summary>
    /// <returns>
    ///	Gets the DateTime this license was last modified.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(null),
    System.ComponentModel.Description("Gets the DateTime this license was last modified."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public DateTime ModificationDate
    {
      get
      {
        if (this.modificationDate.Ticks > 0)
          return this.modificationDate;
        else
          return DateTime.Now;
      }
      set
      {
        this.modificationDate = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// Gets the DateTime this license was first used.
    /// </summary>
    /// <returns>
    ///	Gets the DateTime this license was first used.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(null),
    System.ComponentModel.Description("Gets the DateTime this license was first used."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public DateTime FirstUsesDate
    {
      get
      {
        if (this.firstUseDate.Ticks > 0)
          return this.firstUseDate;
        else
          return DateTime.Now;
      }
      set { firstUseDate = value; }
    }
    /// <summary>
    /// Gets or Sets the <see cref="Issuer"/> object for this license.
    /// </summary>
    /// <param>
    /// Sets the <see cref="Issuer"/> object for this license.
    /// </param>
    /// <returns>
    /// Gets the <see cref="Issuer"/> object for this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(null),
    System.ComponentModel.Description("Gets or Sets the Issuer object for this license."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public Issuer Issuer
    {
      get
      {
        return issuer;
      }
      set
      {
        this.issuer = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// Gets or Sets the Constraints List (System.Collections.Generic.List) for this license.
    /// </summary>
    /// <param>
    ///	Sets the Constraints List (System.Collections.Generic.List) for this license.
    /// </param>
    /// <returns>
    ///	Gets the Constraints List (System.Collections.Generic.List) for this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Gets or Sets the ConstraintCollection for this license."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)
    ]
    public List<IConstraint> Constraints
    {
      get
      {
        return this.constraints;
      }
      set
      {
        this.constraints = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// Returns the value for an item set in the <c>CustomData</c> that matches the entered
    /// key.  If the <c>CustomData</c> has not yet been set then a null exception will be thrown.
    /// Also if the passed in <see cref="System.String">String</see> is empty an argument
    /// exception is thrown.
    /// </summary>
    /// <param>
    /// A <see cref="System.String">String</see> to find the value of the <c>CustomData</c>.
    /// </param>
    /// <return>
    /// The value found matching this key.  Otherwise an empty
    /// <see cref="System.String">String</see> is returned.
    /// </return>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <c>CustomData</c> object has no records.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the input value is an empty <see cref="System.String">String</see>.
    /// </exception>
    public string GetCustomFieldValue(string key)
    {
      if (this.CustomData.Items == null)
      {
        throw new ArgumentNullException("Custom data has not been defined.");
      }
      else if (key != String.Empty)
      {
        foreach (DictionaryEntry dictEntry in this.CustomData.Items)
        {
          if (((string)dictEntry.Key).Equals(key))
            return (string)dictEntry.Value;
        }

        return String.Empty;
      }
      else
        throw new ArgumentException("Empty String is not a valid input parameter.");
    }
    /// <summary>
    /// Gets or Sets the <see cref="CustomData"/> for this license.
    /// </summary>
    /// <param>
    ///	Sets the <see cref="CustomData"/> for this license.
    /// </param>
    /// <returns>
    ///	Gets the <see cref="CustomData"/> for this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("Data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Gets or Sets the CustomData for this license."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnly(true)
    ]
    public CustomData CustomData
    {
      get
      {
        return this.customData;
      }
      set
      {
        this.customData = value;
        this.isDirty = true;
      }
    }
    /// <summary>
    /// HardwareKeyToken Based On Device data (Processor ID and Motherboard device ID)
    /// </summary>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("License upgrade data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("This will generate the Hash key based upon the current computer hardware."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(false)
    ]
    public string HardwareKeyTokenBasedOnDevice
    {
      get
      {
        if ((m_ProcessorId == String.Empty) && (m_MotherboardDeviceID == string.Empty))
          return String.Empty;
        string id = m_ProcessorId + m_MotherboardDeviceID;
        SHA512Managed sha512 = new SHA512Managed();
        byte[] dataBytes = Encoding.ASCII.GetBytes(id);
        sha512.ComputeHash(dataBytes);
        return BitConverter.ToUInt16(sha512.Hash, 0).ToString();
      }
      set { m_HardwareKey = value; }
    }
    /// <summary>
    /// Gets or sets the hardware key token.
    /// this HardwareKeyToken is based on the information int the license
    /// </summary>
    /// <value>The hardware key token.</value>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("License upgrade data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("This will generate the Hash key based upon the current computer hardware."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(false)
    ]
    public string HardwareKeyToken
    {
      get
      {
        return m_HardwareKey;
      }
      set { m_HardwareKey = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("License upgrade data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("This will generate the Hash key based upon the current computer hardware."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(false)
    ]
    public string LicenseKeyToken
    {
      get { return m_LicenseKey; }
      set { m_LicenseKey = value; }
    }
    /// <summary>
    /// "Constrains number of items licensed program can instantiate, e.g. number of process variables, number of interfaces, etc."
    /// </summary>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Limits"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Constrains number of items licensed program can instantiate," +
                                       "e.g. number of process variables, number of interfaces, etc."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(false)
    ]
    public int VolumeConstrain { get { return m_volumeConstrain; } }
    /// <summary>
    /// "Limits the runtime of the program in hours."
    /// </summary>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(true),
    System.ComponentModel.Category("Limits"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("Limits the runtime of the program in hours."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(false)
    ]
    public int RunTimeConstrain { get { return m_runTimeConstrain; } }
    #endregion Properties

    #region License implementation
    /// <summary>
    /// This will generate the Hash key based upon the current installation instance GUID.
    /// </summary>
    /// <returns>
    ///	Gets the license key (XML string representing the license) granted for this license.
    /// </returns>
    [
    System.ComponentModel.Bindable(false),
    System.ComponentModel.Browsable(false),
    System.ComponentModel.Category("License upgrade data"),
    System.ComponentModel.DefaultValue(""),
    System.ComponentModel.Description("This will generate the Hash key based upon the current installation instance GUID."),
    System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible),
    System.ComponentModel.ReadOnlyAttribute(true)
    ]
    public override string LicenseKey
    {
      get
      {
        if (String.IsNullOrEmpty(m_InstallationInstanceGUID))
          return String.Empty;
        using (SHA512Managed sha512 = new SHA512Managed())
        {
          byte[] dataBytes = Encoding.ASCII.GetBytes(m_InstallationInstanceGUID);
          sha512.ComputeHash(dataBytes);
          return BitConverter.ToUInt16(sha512.Hash, 0).ToString();
        }
      }
    }
    #endregion License implementattion

    #region IConstraintItemProvider Members
    List<IConstraint> IConstraintItemProvider.Items
    {
      get { return Constraints; }
    }
    #endregion

    #region IDisposable
    /// <summary>
    /// When overridden in a derived class, disposes of the resources used by the license.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public override void Dispose()
    {
    }
    #endregion

  }
}