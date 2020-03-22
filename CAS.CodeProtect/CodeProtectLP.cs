//<summary>
//  Title   : License provider
//  System  : Microsoft Visual C# .NET 2005
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//  History :
//    MPostol - 10-11-2006
//       Derived from OpenLicenseProvider
//
//  Copyright (C)2006, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto:techsupp@UAOOI.eu
//  http://www.UAOOI.eu
//</summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using UAOOI.CodeProtect.EnvironmentAccess;
using UAOOI.CodeProtect.LicenseDsc;
using UAOOI.CodeProtect.Properties;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace UAOOI.CodeProtect
{
  /// <summary>
  /// This is the core piece to CodeProtect.  It is the piece that the <c>LicenseManager</c>
  /// used to validate that a license can be granted to a specific my_Type.  This provider is
  /// referenced by the process attempting to validate a license.
  /// </summary>
  /// <seealso cref="System.ComponentModel.LicenseProvider">LicenseProvider</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public sealed class CodeProtectLP: LicenseProvider
  {
    #region private

    #region Obtain and save the license
    /// <summary>
    /// Obtains a license from Isolated Storage if exist.
    /// </summary>
    /// <param name="licFile">The string name of the license file.</param>
    /// <param name="key">The encryption key to verify the license.</param>
    /// <param name="licenses">The licenses collection to add new license if exist.</param>
    private void AddLicenseFromIsolatedStorage( string licFile, RSACryptoServiceProvider key, List<License> licenses )
    {
      LicenseFile license = null;
      using ( var reader = IsolatedStorageAccess.CreateIsolatedStorageLicenseStream( FileMode.Open ) )
        license = LicenseFile.LoadFile( reader, key );
      if ( license != null )
        licenses.Add( license );
    }
    /// <summary>
    /// Obtains a license from the application data path and adds it to the <paramref name="licenses"/> if exists.
    /// </summary>
    /// <param name="key">The encryption key to verify the license.</param>
    /// <param name="licenses">The licenses collection.</param>
    private void AddLicenseFromPaths( RSACryptoServiceProvider key, List<License> licenses )
    {
      LicenseFile lic = null;
      using ( Stream stream = FileNames.CreateLicenseFileStream( FileMode.Open, FileAccess.Read, FileShare.Read ) )
        lic = LicenseFile.LoadFile( stream, key );
      if ( lic != null )
        licenses.Add( lic );
    }
    private LicenseFile GetLicense( Type pType, RSACryptoServiceProvider key )
    {
      StringBuilder AnyErrorInformation = new StringBuilder();
      LicenseFile currentLicense = null;
      List<License> licenses = new List<License>();
      string licFile = FileNames.LicenseFileName;
      // Look for the license in the cache otherwise get it from AppData or Isolated Storage
      try
      {
        if ( CheckIfWebApplication() )
          WebApplicationLicenseCollector.AddLicenseFromCollector( pType, licenses );
      }
      catch ( Exception ex )
      {
        AnyErrorInformation.Append( Resources.ErrStr_GetLicensee_UnableToGetLicenseFrom_web );
        AnyErrorInformation.AppendLine( ex.Message );
      }
      try
      {
        AddLicenseFromIsolatedStorage( licFile, key, licenses );
      }
      catch ( Exception ex )
      {
        AnyErrorInformation.Append( Resources.ErrStr_GetLicensee_UnableToGetLicenseFrom_isolatedstorage );
        AnyErrorInformation.AppendLine( ex.Message );
      }
      try
      {
        AddLicenseFromPaths( key, licenses );
      }
      catch ( Exception ex )
      {
        AnyErrorInformation.Append( Resources.ErrStr_GetLicensee_UnableToGetLicenseFrom_default );
        AnyErrorInformation.AppendLine( ex.Message );
      }
      bool requiredToGetNewestLicense = false;
      //Compare currentLicense with Alternate License
      if ( licenses.Count > 0 )
      {
        foreach ( LicenseFile item in licenses )
        {
          if ( currentLicense == null )
            currentLicense = item;
          else
            if ( currentLicense.LicenseUID != item.LicenseUID ||
              currentLicense.LicModificationUID != item.LicModificationUID )
            {
              requiredToGetNewestLicense = true;
              break;
            }
            else if ( currentLicense.Statistics.HitCount < item.Statistics.HitCount )
              //Replace the current license since the alternate is older (in the mean of hit count).
              currentLicense = item;
        }
        if ( requiredToGetNewestLicense )
          foreach ( LicenseFile item in licenses )
            if ( currentLicense == null || currentLicense.ModificationDate < item.ModificationDate )
              //Replace the current license since the alternate is older.
              currentLicense = item;
      }
      else
        throw new LicenseFileException( AnyErrorInformation.ToString() );
      return currentLicense;
    }
    /// <summary>
    /// This is responsible for saving the license into the proper locations.
    /// </summary>
    /// <param name="type">The object <c>Type</c> being licensed.</param>
    /// <param name="license">The license to save.</param>
    /// <param name="key">The encryption key to verify the license.</param>
    private void Save( Type type, LicenseFile license, RSACryptoServiceProvider key )
    {
      Debug.WriteLine( "OpenLicenseProvider: PerformSave function" );
      if ( CheckIfWebApplication() ) // Web app
      {
        //Force the save outside the bin directory since this causes the app to restart.
        string absoluteFilePath = HttpContext.Current.Server.MapPath( "~" + Path.DirectorySeparatorChar ) + license.Product.LicFileName;
        Save( license, key, false, absoluteFilePath );
        WebApplicationLicenseCollector.AddLicense2Collector( type, license );
      }
      else
        Save( license, key, true, FileNames.ApplicationDataPath );
    }
    /// <summary>
    /// This saves the license in the proper location.
    /// </summary>
    /// <param name="licenseFile">The license file to save.</param>
    /// <param name="key">The encryption key to verify the license.</param>
    /// <param name="useIsolatedStorage">Save to IsolatedStorage if true if saving in the application data path causes an exception.</param>
    /// <param name="appDataFilePath">The application data file path.</param>
    private void Save
      ( LicenseFile licenseFile, RSACryptoServiceProvider key, bool useIsolatedStorage, string appDataFilePath )
    {
      Debug.WriteLine( "OpenLicenseProvider: Save Function" );
      try
      {
        using ( Stream fileStream = FileNames.CreateLicenseFileStream( FileMode.Create, FileAccess.Write, FileShare.Write ) )
          licenseFile.SaveFile( fileStream, key );
      }
      catch ( Exception ex )
      {
        if ( !useIsolatedStorage )
          throw new LicenseFileException( Resources.ErrStr_SaveLicense_UnableToSaveLicense + ex.Message );
      }
      if ( useIsolatedStorage )
      {
        using ( Stream writer = IsolatedStorageAccess.CreateIsolatedStorageLicenseStream( FileMode.Create ) )
          licenseFile.SaveFile( writer, key );
      }
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Used to build an exception string with the Helper Attributes data if it exists.
    /// </summary>
    /// <param name="type">The object type that the exception occurred for.</param>
    /// <param name="protectedAss">The assembly containing definition of the <paramref name="type"/>.</param>
    /// <param name="ex">The exception to be documented.</param>
    /// <returns>
    /// Formated message with the Helper Attributes data if it exists.
    /// </returns>
    private string BuildExceptionString( Type type, Assembly protectedAss, Exception ex )
    {
      Debug.WriteLine( "OpenLicenseProvider: BuildExceptionString Function" );
      StringBuilder errorMsg = new StringBuilder();
      errorMsg.AppendLine( Resources.LicenseFileErrorIntro );
      errorMsg.AppendLine( ex.Message );
      string typeName = type == null ? Resources.LicenseFileErrorTypeUnknown : type.ToString();
      errorMsg.Append( String.Format( Resources.LicenseFileErrorAValidLicense, typeName ) );
      AssemblyHelperAttribute attr = null;
      if ( protectedAss != null )
        attr = (AssemblyHelperAttribute)Attribute.GetCustomAttribute( protectedAss, typeof( AssemblyHelperAttribute ) );
      if ( attr != null )
      {
        if ( !String.IsNullOrEmpty( attr.Product ) )
          errorMsg.Append( attr.Product );
        else
          errorMsg.Append( GetProductNameFromAssembly( type, protectedAss ) );
        errorMsg.AppendLine( "." );
        errorMsg.AppendLine( attr.ToString() );
      }
      else
      {
        errorMsg.Append( GetProductNameFromAssembly( type, protectedAss ) );
        errorMsg.AppendLine( "." );
        errorMsg.AppendLine( Resources.LicenseFileErrorContactManufacturer );
      }
      return errorMsg.ToString();
    }
    private static string GetProductNameFromAssembly( Type type, Assembly protectedAss )
    {
      if ( ( protectedAss == null ) && ( type != null ) )
        protectedAss = Assembly.GetAssembly( type );
      if ( protectedAss == null )
        protectedAss = Assembly.GetCallingAssembly();
      return protectedAss.GetName().FullName;
    }
    private bool CheckIfWebApplication()
    {
      return HttpContext.Current != null;
    }
    #endregion
    #endregion private

    #region creators
    /// <summary>
    /// This is the constructor for the <c>OpenLicenseProvider</c>.
    /// </summary>
    public CodeProtectLP()
    {
      Debug.WriteLine( "OpenLicenseProvider: OpenLicenseProvider Constructor" );
    }
    #endregion

    #region public
    /// <summary>
    /// Reads the deploy manifest.
    /// </summary>
    /// <param name="ManifestFileName">Name of the manifest file.</param>
    /// <returns>The DeployManifest readed from XML File</returns>
    public static DeployManifest ReadDeployManifest( string ManifestFileName )
    {
      return ManifestManagement.ReadDeployManifest( ManifestFileName );
    }
    /// <summary>
    /// Gets the opening error from last opening operation.
    /// </summary>
    /// <value>The opening error.</value>
    public static string OpeningError { get; private set; }
    #endregion

    #region LicenseProvider implementation
    /// <summary>
    /// Gets a license for an instance or type of component, when given a context and whether the denial of a license
    /// throws an exception. This is responsible for obtaining the license or reporting the error if non is found.
    /// </summary>
    /// <param name="context">A <see cref="System.ComponentModel.LicenseContext"/> that specifies where you can use the licensed object.</param>
    /// <param name="type">A <see cref="System.Type"/> that represents the component requesting the license.</param>
    /// <param name="instance">An object that is requesting the license.</param>
    /// <param name="allowExceptions">true if a <see cref="System.ComponentModel.LicenseException"/> should be thrown when the
    /// component cannot be granted a license; otherwise, false..</param>
    /// <returns>
    /// A valid <see cref="System.ComponentModel.License"/>.
    /// </returns>
    /// <exception name="LicenseException">
    /// 	<p>The reason the validation failed.</p>
    /// </exception>
    public override License GetLicense( LicenseContext context, Type type, object instance, bool allowExceptions )
    {
      Assembly protectedAss = null;
      LicenseFile currentLicense = null;
      try
      {
        Debug.WriteLine( "OpenLicenseProvider: GetLicense Function" );
        if ( context == null )
          throw new ArgumentNullException( Resources.LicenseFileErrorContext );
        if ( type == null )
          throw new ArgumentNullException( Resources.LicenseFileErrorType );
        protectedAss = Assembly.GetAssembly( type );
        if ( protectedAss == null )
          throw new LicenseFileException( string.Format( Resources.LicenseFileErrorAssembly, type.ToString() ) );
        RSACryptoServiceProvider key = null;
        try
        {
          key = CodeProtectHelpers.ReadKeysFromProtectedArea( CodeProtectHelpers.GetEntropy() );
        }
        catch ( Exception ex )
        {
          throw new LicenseFileException( Resources.LicenseFileErrorRSA, ex );
        }
        currentLicense = GetLicense( type, key );
        if ( !currentLicense.ValidateLicense( type ) )
        {
          if ( String.IsNullOrEmpty( currentLicense.FailureReason ) )
            currentLicense.FailureReason = Resources.LicMessageFunctionNoReason;
          throw new LicenseFileException( currentLicense.FailureReason );
        }
        if ( ( currentLicense.Statistics.DateTimeLastAccessed.Ticks > 0 ) &&
          ( currentLicense.Statistics.DateTimeLastAccessed.CompareTo( DateTime.Now ) > 0 ) )
        {
          currentLicense.FailureReason = Resources.LicMessageFunctionDataTime;
          throw new LicenseFileException( currentLicense.FailureReason );
        }
        //Ok we have a valid license now...
        //Lets update the stats and save this file...
        currentLicense.Statistics.UpdateLastAccessDate();
        currentLicense.Statistics.IncrementDaysUsed();
        currentLicense.Statistics.IncrementAccessCount();
        currentLicense.Statistics.IncrementUsageUsed();
        currentLicense.Statistics.IncrementHitCount();
        Save( type, currentLicense, key );
        return currentLicense;
      }
      catch ( Exception ex )
      {
        OpeningError = BuildExceptionString( type, protectedAss, ex );
        if ( allowExceptions )
          throw new LicenseException( type, instance, OpeningError );
        else
          return currentLicense;
      }
    }
    #endregion
  }
}
