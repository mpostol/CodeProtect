//<summary>
//  Title   : Context keys used to get property form the insaller CustomActionData
//  System  : Microsoft Visual C# .NET 2010
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

using CAS.Lib.CodeProtect.EnvironmentAccess;

namespace CAS.Lib.CodeProtect
{
  /// <summary>
  /// Context keys used to get property form the insaller CustomActionData and additional important instalation information
  /// </summary>
  /// <remarks>To get description of properties provided by the installer visit the msdn page:
  /// http://msdn.microsoft.com/en-us/library/aa370905(v=VS.85).aspx
  /// </remarks>
  public static class InstallContextNames
  {
    #region internal
    //Propierties defined by the user.
    internal const string User = "ca_user";
    internal const string Company = "ca_company";
    internal const string Email = "ca_email";
    //Installer properties 
    internal const string Productname = "ca_productname";
    internal const string Version = "ca_version";
    internal const string Allusers = "ca_allusers";
    internal const string Upgradecode = "ca_upgradecode";
    internal const string Manufacturer = "ca_manufacturer";
    internal const string Arphelplink = "ca_arphelplink";
    internal const string DefaultPublisherName = "CAS";
    #endregion internal
    #region public
    /// <summary>
    /// Gets the application data directory path.
    /// </summary>
    /// <value>The application data directory path.</value>
    public static string ApplicationDataPath
    {
      get
      {
        return FileNames.ApplicationDataPath;
      }
    }
    #endregion public
  }
}
