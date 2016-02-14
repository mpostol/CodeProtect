//<summary>
//  Title   : This is a collection of Licenses used internally by the provider when caching license files.
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;

namespace CAS.Lib.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// This is a collection of Licenses used internally by the provider when caching
  /// license files. Cache is only used in web environments.
  /// </summary>
  internal sealed class WebApplicationLicenseCollector
  {
    #region internal
    ///<summary>
    /// This adds a new license to the collector.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type">Type</see> of this object to create the license for.
    /// </param>
    /// <param name="license">
    /// The license to add.
    /// </param>
    /// <exception name="ArgumentNullException">
    /// <p>Thrown if the license or type value are null</p>
    /// </exception>
    internal static void AddLicense2Collector( Type type, License license )
    {
      if ( type == null )
        throw new ArgumentNullException( "GetLicense requires that the parameter type is not null" );
      if ( license == null )
        throw new ArgumentNullException( "GetLicense requires that the parameter license is not null" );
      LicenseCollector.collectedLicenses[ type ] = license;
    }
    ///<summary>
    /// This gets the license for the given <see cref="Type">Type</see>.
    /// </summary>
    /// <param>
    /// The <see cref="Type">Type</see> to get the license for.
    /// </param>
    /// <exception name="ArgumentNullException">
    /// <p>Thrown if the my_Type value are null</p>
    /// </exception>
    internal static void AddLicenseFromCollector( Type type, List<License> licenses )
    {
      if ( type == null )
        throw new ArgumentNullException( "GetLicense requires that the parameter type is not null" );
      if ( LicenseCollector.collectedLicenses.Count == 0 )
        return;
      License lic = LicenseCollector.collectedLicenses[ type ] as License;
      if ( lic != null )
        licenses.Add( lic );
    }
    #endregion
    #region private
    private static readonly WebApplicationLicenseCollector m_LicenseCollector = new WebApplicationLicenseCollector();
    private IDictionary collectedLicenses;
    ///<summary>
    /// This is the constructor for the OpenLicenseCollector.
    /// </summary>
    private WebApplicationLicenseCollector()
    {
      collectedLicenses = new HybridDictionary();
    }
    /// <summary>
    /// Gets the LicenseCollector from the current location.  Used to access the proper context
    /// for the License Collection.
    /// </summary>
    private static WebApplicationLicenseCollector LicenseCollector
    {
      get
      {
        Debug.WriteLine( "Get Static LicenseCollector" );
        WebApplicationLicenseCollector localLC;
        Debug.Assert( HttpContext.Current == null, "Cache should be only used in web environments." );
        localLC = (WebApplicationLicenseCollector)HttpContext.Current.Application[ "LicenseCollector" ];
        if ( localLC == null )   //Should occur only on first application usage
        {
          localLC = m_LicenseCollector;
          HttpContext.Current.Application.Add( "LicenseCollector", localLC );
          //TimeStamp = DateTime.Now;
        }
        return localLC;
      }
    } //LicenseCollector Property
    #region ??????
    /////<summary>
    // /// This removes the license for the given <see cref="Type">Type</see>.
    // /// </summary>
    // /// <param>
    // /// The <see cref="Type">Type</see> to remove.
    // /// </param>
    // /// <exception name="ArgumentNullException">
    // /// <p>Thrown if the my_Type value are null</p>
    // /// </exception>
    // private static void RemoveLicense( Type type )
    // {
    //   if ( type == null )
    //     throw new ArgumentNullException( "GetLicense requires that the parameter type is not null" );
    //   LicenseCollector.collectedLicenses.Remove( type );
    // }
    // /// <summary>
    // /// Removes the instance of <see cref="Type">Type</see> from the cache. Cache is
    // /// only used in web environments.  This call should only be used if you want to force
    // /// Open License to read the file from the disk.
    // /// </summary>
    // /// <param name="type">
    // /// The <see cref="Type">Type</see> to remove from the cache.
    // /// </param>
    // /// <remarks>
    // /// It is highly recommended that this never be used in real world environments because
    // /// the cache is where all settings are kept. So if the cache is reset then the license will never
    // /// expire for Usage my_Type constraints.
    // /// </remarks>
    // private static void ResetCashe( Type type )
    // {
    //   Utilities.WriteDebugOutput( "OpenLicenseProvider: ResetCache : " + type.ToString() );
    //   RemoveLicense( type );
    // }
    //private static DateTime TimeStamp
    //{
    //  get
    //  {
    //    object ts = HttpContext.Current.Application[ "LicenseCollectorTS" ];
    //    if ( ts == null )   //Should not occur
    //    {
    //      ts = DateTime.Now;
    //      HttpContext.Current.Application.Add( "LicenseCollectorTS", (DateTime)ts );
    //    }
    //    return (DateTime)ts;
    //  }
    //  set
    //  {
    //    object ts = HttpContext.Current.Application[ "LicenseCollectorTS" ];
    //    if ( ts == null )
    //      HttpContext.Current.Application.Add( "LicenseCollectorTS", value );
    //    else
    //      HttpContext.Current.Application[ "LicenseCollectorTS" ] = value;
    //  }
    //}//TimeStamp Property
    #endregion
    #endregion
  }
}
