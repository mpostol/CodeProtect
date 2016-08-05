//<summary>
//  Title   : Provides the foundation for custom installations.
//  System  : Microsoft Visual C# .NET 2005
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//  History :
//    MPostol 06-03-2007: 
//      created;
//
//  Copyright (C)2006-2011, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto:techsupp@cas.com.pl
//  http://www.cas.eu
//</summary>

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using CAS.Lib.CodeProtect.EnvironmentAccess;
using CAS.Lib.CodeProtect.LicenseDsc;

namespace CAS.Lib.CodeProtect
{
  /// <summary>
  /// Provides the foundation for custom installations.
  /// </summary>
  /// <remarks>It installs manifest and signed by the application a demo license file</remarks>
  [RunInstaller( true )]
  public partial class LibInstaller: Installer
  {
    #region creator
    /// <summary>
    /// Costructor of the custom installation class;
    /// </summary>
    public LibInstaller()
      : base()
    {
      InitializeComponent();
    }
    #endregion
    #region public static
    /// <summary>
    /// Installs the license for the component with the default values for the user of the license. Could be used while debugging to 
    /// allow to pass the license validation process. 
    /// </summary>
    /// <remarks>This method is obsolete and will be removed in the future - use InstalLicense( LoadLicenseFromDefaultContainer = false) instead.</remarks>
    [Obsolete( "This method is obsolete and will be removed in the future - use InstalLicense( LoadLicenseFromDefaultContainer = false) instead." )]
    public static void InstalLicense()
    {
      InstalLicense( false );
    }
    /// <summary>
    /// Installs the license for the component with the default values for the user of the license. Could be used while debugging to 
    /// allow to pass the license validation process. 
    /// </summary>
    /// <param name="LoadLicenseFromDefaultContainer">if set to <c>true</c>  
    /// the license is loaded from default container.
    /// otherwise license is loaded from file</param>
    public static void InstalLicense( bool LoadLicenseFromDefaultContainer )
    {
      InstalLicense( Environment.UserName, Environment.MachineName, Environment.UserName + "@" + Environment.UserDomainName, LoadLicenseFromDefaultContainer );
    }
    /// <summary>
    /// Installs the license for the component. Could be used for ClickOnce deployment
    /// to install the license while installing  the software.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="company">The company.</param>
    /// <param name="email">The email.</param>
    /// <param name="LoadLicenseFromDefaultContainer">if set to <c>true</c>  
    /// the license is loaded from default container.
    /// otherwise license is loaded from file</param>
    public static void InstalLicense( string user, string company, string email, bool LoadLicenseFromDefaultContainer )
    {
      InstalLicense( user, company, email, LoadLicenseFromDefaultContainer, null, null );
    }
    internal static void InstalLicense( string user, string company, string email, bool LoadLicenseFromDefaultContainer, string AlternativeProductName, string LicenseUnlockCode )
    {
      //Depending on the environment we can get null for some Assembly.Get..
      Assembly assembly = Assembly.GetEntryAssembly();
      if ( assembly == null )
        assembly = Assembly.GetCallingAssembly();
      if ( assembly == null )
        assembly = Assembly.GetExecutingAssembly();
      ManifestManagement.WriteDeployManifest( assembly, AlternativeProductName );
      LicenseFile.Instal( user, company, email, FileNames.LicenseFilePath, LoadLicenseFromDefaultContainer, LicenseUnlockCode );
    }
    /// <summary>
    /// Instals the license fro the <see cref="Stream"/>.
    /// </summary>
    /// <param name="license">The license available as the <see cref="Stream"/>.</param>
    public static void InstalLicense( Stream license )
    {
      LicenseFile.Instal( license );
    }
    #endregion
    #region Installer inmplementation
    /// <summary>
    /// Perform the installation. 
    /// </summary>
    /// <param name="stateSaver">
    /// An IDictionary used to save information needed to perform a commit, rollback, or uninstall operation. 
    /// </param>
    public override void Install( System.Collections.IDictionary stateSaver )
    {
      base.Install( stateSaver );
    }
    /// <summary>
    /// When overridden in a derived class, completes the install transaction.
    /// </summary>
    /// <param name="savedState">An <see cref="T:System.Collections.IDictionary"/> that contains the state 
    /// of the computer after all the installers in the collection have run.
    /// </param>
    /// <exception cref="T:System.ArgumentException">
    /// The <paramref name="savedState"/> parameter is null.
    /// -or-
    /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
    /// </exception>
    /// <exception cref="T:System.Configuration.Install.InstallException">
    /// An exception occurred during the <see cref="M:System.Configuration.Install.Installer.Commit(System.Collections.IDictionary)"/> phase 
    /// of the installation. This exception is ignored and the installation continues. 
    /// However, the application might not function correctly after the installation is complete.
    /// </exception>
    public override void Commit( IDictionary savedState )
    {
      base.Commit( savedState );
      InstallContext m_Cnt = this.Context;
      try
      {
        CreateDeployManifest( m_Cnt );
        LicenseFile.Instal
          ( m_Cnt.Parameters[ InstallContextNames.User ],
            m_Cnt.Parameters[ InstallContextNames.Company ],
            m_Cnt.Parameters[ InstallContextNames.Email ],
            FileNames.LicenseFilePath,
            true,
            null
          );
      }
      catch ( Exception ex )
      {
        throw new InstallException( "Installation Error", ex );
      }
    }
    /// <summary>
    /// When overridden in a derived class, restores the pre-installation state of the computer.
    /// </summary>
    /// <param name="savedState">An <see cref="T:System.Collections.IDictionary"/> that contains the pre-installation state of the computer.</param>
    /// <exception cref="T:System.ArgumentException">
    /// The <paramref name="savedState"/> parameter is null.
    /// -or-
    /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
    /// </exception>
    /// <exception cref="T:System.Configuration.Install.InstallException">
    /// An exception occurred during the <see cref="M:System.Configuration.Install.Installer.Rollback(System.Collections.IDictionary)"/> phase 
    /// of the installation. This exception is ignored and the rollback continues. 
    /// However, the computer might not be fully reverted to its initial state after the rollback completes.
    /// </exception>
    public override void Rollback( IDictionary savedState )
    {
      LicenseFile.Uninstal();
      FileNames.DeleteKeys();
      ManifestManagement.DeleteDeployManifest();
      base.Rollback( savedState );
    }
    /// <summary>
    /// Removes an installation. 
    /// </summary>
    /// <param name="savedState">
    /// An IDictionary used to save information needed to perform a commit, rollback, or uninstall operation. 
    /// </param>
    public override void Uninstall( System.Collections.IDictionary savedState )
    {
      LicenseFile.Uninstal();
      FileNames.DeleteKeys();
      ManifestManagement.DeleteDeployManifest();
      base.Uninstall( savedState );
    }
    #endregion
    #region private
    private void CreateDeployManifest( InstallContext m_Cnt )
    {
      string productName = m_Cnt.Parameters[ InstallContextNames.Productname ];
      Version version = new Version( m_Cnt.Parameters[ InstallContextNames.Version ] );
      string allUsers = m_Cnt.Parameters[ InstallContextNames.Allusers ];
      AssemblyName an = new AssemblyName( productName )
      {
        Version = version,
        Flags = AssemblyNameFlags.None,
        CultureInfo = System.Globalization.CultureInfo.InvariantCulture
      };
      //it is public token from cas.snk
      an.SetPublicKeyToken( new byte[] { 0x88, 0x32, 0xff, 0x1a, 0x67, 0xea, 0x61, 0xa3 } );
      ManifestManagement.ProductType type =
        String.Compare( allUsers, "1" ) == 0 ? ManifestManagement.ProductType.AllUsers : ManifestManagement.ProductType.SingleUser;
      string publisher = m_Cnt.Parameters[ InstallContextNames.Manufacturer ];
      Uri supportUrl = new Uri( m_Cnt.Parameters[ InstallContextNames.Arphelplink ] );
      string appData = FileNames.ConstructApplicationDataFolder( publisher, productName );
      if ( !( new DirectoryInfo( appData ) ).Exists )
        throw new InstallException( appData + " - directory for application data - does not exists" );
      SetModifyRights( appData );
      ManifestManagement.WriteDeployManifest( an, type, publisher, supportUrl, supportUrl, appData );
    }
    private static void SetModifyRights( string appData )
    {
      DirectorySecurity ds = Directory.GetAccessControl( appData );
      SecurityIdentifier identity = new SecurityIdentifier( WellKnownSidType.BuiltinUsersSid, null );
      FileSystemRights rights = FileSystemRights.Modify | FileSystemRights.CreateFiles;
      FileSystemAccessRule rules = new FileSystemAccessRule
        ( identity, rights, InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow );
      ds.AddAccessRule( rules );
      Directory.SetAccessControl( appData, ds );
    }
    #endregion
  }
}