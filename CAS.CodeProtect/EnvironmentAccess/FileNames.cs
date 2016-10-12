//<summary>
//  Title   : Provides static method to get access to common files: manifest, license, etc.
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2010, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace CAS.Lib.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Provides static method to get access to common files: manifest, license, etc.
  /// </summary>
  public static class FileNames
  {
    #region internal
    internal static string ConstructApplicationDataFolder(string Manufacturer, string ProductName)
    {
      // the folowing directory construction must match Application Data Folder in deployment project
      StringBuilder sb = new StringBuilder();
      sb.Append(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
      sb.Append("\\");
      sb.Append(Manufacturer);
      sb.Append("\\");
      sb.Append(ProductName);
      sb.Append("\\");
      return sb.ToString();
    }
    /// <summary>
    /// Gets or sets the target dir where this assembly is located.
    /// </summary>
    /// <value>The target dir.</value>
    internal static string TargetDir { get; set; }
    /// <summary>
    /// Gets or sets the name and full path of the manifest file.
    /// </summary>
    /// <value>The name of the manifest file.</value>
    internal static string ManifestFileName { get; private set; }
    /// <summary>
    /// Gets the application data dir.
    /// </summary>
    /// <value>The application data directory.</value>
    internal static string ApplicationDataPath { get { return ManifestHandling.ApplicationData; } }
    /// <summary>
    /// Path of the file containing keys pair.
    /// </summary>
    /// <returns></returns>
    internal static string KeysFilePath()
    {
      return Path.Combine(TargetDir, m_ParKeysFileName);
    }
    /// <summary>
    /// Gets the license file name with full path.
    /// </summary>
    /// <value>The license file path.</value>
    internal static string LicenseFilePath
    {
      get { return Path.Combine(ApplicationDataPath, LicenseFileName); }
    }
    /// <summary>
    /// If exists removes keys file 
    /// </summary>
    internal static void DeleteKeys()
    {
      if (!File.Exists(KeysFilePath()))
        return;
      FileInfo keysFileInfo = new FileInfo(FileNames.KeysFilePath());
      keysFileInfo.Attributes = FileAttributes.Normal;
      File.Delete(KeysFilePath());
    }
    /// <summary>
    /// Creates the license file stream.
    /// </summary>
    /// <param name="fileMode">The file mode.</param>
    /// <param name="fileAccess">The file access.</param>
    /// <param name="fileShare">The file share.</param>
    /// <returns>An object of <see cref="Stream"/> to be used to obtail the license</returns>
    /// <exception cref="System.IO.FileNotFoundException"> The file cannot be found, such as when mode is 
    /// FileMode.Truncate or FileMode.Open, and the file specified by path does not exist. The file must 
    /// already exist in these modes.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    public static Stream CreateLicenseFileStream(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
    {
      string filePath = Path.Combine(ApplicationDataPath, LicenseFileName);
      return new FileStream(filePath, fileMode, fileAccess, fileShare);
    }
    /// <summary>
    /// This products manifest.
    /// </summary>
    /// <returns></returns>
    internal static DeployManifest ProductManifest()
    {
      return ManifestHandling.DeployManifest;
    }
    internal static void UnloadProductManifest()
    {
      ManifestHandling.UnloadManifest();
    }
    #endregion

    #region public
    /// <summary>
    /// Extension of License file
    /// </summary>
    public const string LicExtension = "lic";
    /// <summary>
    /// Returns file name with extension for the licence file.
    /// </summary>
    /// <returns>File name of the license file</returns>
    public static string LicenseFileName { get { return "CAS.License." + LicExtension; } }
    #endregion

    #region private
    static FileNames()
    {
      TargetDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      ManifestFileName = Path.Combine(FileNames.TargetDir, m_ManifestFileName);
    }
    private const string m_ParKeysFileName = "CAS.Product.ppk";
    private const string m_ManifestFileName = "CAS.Product.xml";
    private static class ManifestHandling
    {
      internal static string ApplicationData { get; private set; }
      internal static DeployManifest DeployManifest
      {
        get
        {
          if (mDeployManifest == null)
            Initialization();
          return mDeployManifest;
        }
      }
      internal static void UnloadManifest()
      {
        mDeployManifest = null;
      }
      #region private
      private static DeployManifest mDeployManifest;
      static ManifestHandling()
      {
        Initialization();
      }
      private static void Initialization()
      {
        //This is called in static creator. 
        //It is possible that this might be called before creation of valid manifest, 
        //that’s why it is worth to call Initialization again when the DeployManifest is null.
        //see property "DeployManifest"
        mDeployManifest = ManifestManagement.ReadDeployManifest();
        ApplicationData = GetApplicationData();
      }
      private static string GetApplicationData()
      {
        string path;
        if (mDeployManifest == null)
        {
          // there is no valid manifest we have to return folder where application executables are stored.
          // this may happened during DEBUG session or when sb. delete or destroy the manifest
          path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        else
          path = DeployManifest.FileReferences[0].TargetPath;
        return path;
      }
      #endregion private
    }
    #endregion
  }
}