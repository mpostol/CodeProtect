//___________________________________________________________________________________
//
//  Copyright (C) 2020, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace UAOOI.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Provides the Isolated Storage access functionality.
  /// </summary>
  internal static class IsolatedStorageAccess
  {
    #region internal

    /// <summary>
    /// Creates a stream to read the License from from Isolated Storage.  Throws an exception if the file doesn't exist.
    /// </summary>
    /// <param name="fileMode">The file mode - one of the <see cref="System.IO.FileMode"/> values.</param>
    /// <returns>
    /// An object of <see cref="IsolatedStorageFileStream"/> if license files exists in the store and can be
    /// opened using <paramref name="fileMode"/>.
    /// </returns>
    /// <exception cref="System.IO.FileNotFoundException">No file was found and the mode is set to <see cref="System.IO.FileMode.Open"/>.
    /// </exception>
    internal static IsolatedStorageFileStream CreateIsolatedStorageLicenseStream(FileMode fileMode)
    {
      Debug.WriteLine("OpenLicenseProvider: GetIsolatedStorageLicense Function");
      System.Diagnostics.Debug.Assert(HttpContext.Current == null, "Web application must not use the Isolated Storage"); //Web App
      IsolatedStorageFile isoFile;
      try
      {
        isoFile = IsolatedStorageFile.GetStore(m_Scope, null, null);
      }// it throws here:IsolatedStorageException: Unable to determine the identity of domain
      catch
      {
        isoFile = IsolatedStorageFile.GetUserStoreForAssembly();
      }
      IFormatter formatter = new BinaryFormatter();
      string licenseString = string.Empty;
      string isoFileName = GenerateIsolatedFileName();
      return new IsolatedStorageFileStream(isoFileName, fileMode, isoFile);
    }

    #endregion internal

    #region private

    private const IsolatedStorageScope m_Scope = IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly;

    /// <summary>
    /// MPTD zapis w application data wymaga generowania unikalnych nazw podobnie jak w innych wspólnych miejscach.
    /// Returns the hash key based upon the passed in full file name.
    /// </summary>
    /// <returns>
    /// The generated file name as a <see cref="System.String">String</see> from hash computed against the <see cref="FileNames.LicenseFileName"/>.
    /// </returns>
    private static string GenerateIsolatedFileName()
    {
      Debug.WriteLine("OpenLicenseProvider: GenerateIsolatedFileName Function");
      byte[] dataBytes = Encoding.UTF8.GetBytes(FileNames.LicenseFileName);
      using (SHA1Managed sha1 = new SHA1Managed())
      {
        sha1.ComputeHash(dataBytes);
        return BitConverter.ToString(sha1.Hash).Replace("-", "");
      }
    }

    #endregion private
  }
}