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

using CAS.Lib.CodeProtect.EnvironmentAccess;
using CAS.Lib.CodeProtect.LicenseDsc;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;

namespace CAS.Lib.CodeProtect
{
  /// <summary>
  /// Provides the foundation for custom installations.
  /// </summary>
  /// <remarks>It installs manifest and signed by the application a demo license file</remarks>
  [RunInstaller(true)]
  public partial class LibInstaller : Installer
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
    /// <param name="LoadLicenseFromDefaultContainer">if set to <c>true</c>
    /// the license is loaded from default container.
    /// otherwise license is loaded from file</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Source Assembly cannot be found.</exception>
    public static void InstallLicense(bool LoadLicenseFromDefaultContainer)
    {
      Assembly assembly = Assembly.GetEntryAssembly(); //For the Unit Tests it returns null
      if (assembly == null)
        assembly = Assembly.GetCallingAssembly();
      if (assembly == null)
        throw new ArgumentOutOfRangeException("Source Assembly", Properties.Resources.SourceAssemblyFailedMessage);
      InstallLicense(Environment.UserName, Environment.MachineName, Environment.UserName + "@" + Environment.UserDomainName, LoadLicenseFromDefaultContainer, null, null, assembly);
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
    /// <exception cref="System.ArgumentOutOfRangeException">Source Assembly cannot be found</exception>
    public static void InstallLicense(string user, string company, string email, bool LoadLicenseFromDefaultContainer)
    {
      Assembly assembly = Assembly.GetEntryAssembly(); //For the Unit Tests it returns null
      if (assembly == null)
        assembly = Assembly.GetCallingAssembly();
      if (assembly == null)
        throw new ArgumentOutOfRangeException("Source Assembly", Properties.Resources.SourceAssemblyFailedMessage);
      InstallLicense(user, company, email, LoadLicenseFromDefaultContainer, null, null, assembly);
    }
    internal static void InstallLicense(string user, string company, string email, bool loadLicenseFromDefaultContainer, string alternativeProductName, string licenseUnlockCode, Assembly assembly)
    {
      if (assembly == null)
        throw new ArgumentNullException(nameof(assembly), $"Entring {nameof(InstallLicense)} with null argument.");
      LicenseTraceSource.TraceVerbose(39, $"Entering InstallLicense {user}, {company}, {email}");
      ManifestManagement.WriteDeployManifest(assembly, alternativeProductName);
      LicenseFile.Install(user, company, email, FileNames.LicenseFilePath, loadLicenseFromDefaultContainer, licenseUnlockCode);
    }
    /// <summary>
    /// Installs the license fro the <see cref="Stream"/>.
    /// </summary>
    /// <param name="license">The license available as the <see cref="Stream"/>.</param>
    public static void InstallLicense(Stream license)
    {
      LicenseFile.Install(license);
    }
    #endregion

    #region Installer inmplementation
    /// <summary>
    /// Perform the installation. 
    /// </summary>
    /// <param name="savedState">
    /// An IDictionary used to save information needed to perform a commit, rollback, or uninstall operation. 
    /// </param>
    public override void Install(IDictionary savedState)
    {
      base.Install(savedState);
    }
    /// <summary>
    /// It completes the install transaction.
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
    public override void Commit(IDictionary savedState)
    {
      base.Commit(savedState);
      InstallContext _Context = this.Context;
      try
      {
        ManifestManagement.WriteDeployManifest(_Context);
        LicenseFile.Install(_Context.Parameters[InstallContextNames.User], _Context.Parameters[InstallContextNames.Company], _Context.Parameters[InstallContextNames.Email], FileNames.LicenseFilePath, true, null);
      }
      catch (Exception ex)
      {
        throw new InstallException("Installation Error", ex);
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
    public override void Rollback(IDictionary savedState)
    {
      LicenseFile.Uninstall();
      FileNames.DeleteKeys();
      ManifestManagement.DeleteDeployManifest();
      base.Rollback(savedState);
    }
    /// <summary>
    /// Removes an installation. 
    /// </summary>
    /// <param name="savedState">
    /// An IDictionary used to save information needed to perform a commit, rollback, or uninstall operation. 
    /// </param>
    public override void Uninstall(System.Collections.IDictionary savedState)
    {
      LicenseFile.Uninstall();
      FileNames.DeleteKeys();
      ManifestManagement.DeleteDeployManifest();
      base.Uninstall(savedState);
    }
    #endregion

  }
}