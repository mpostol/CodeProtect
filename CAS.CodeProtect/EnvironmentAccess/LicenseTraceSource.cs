//<summary>
//  Title   : CAS.Lib.CodeProtect.EnvironmentAccess.LicenseTraceSource - local trace source 
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2011, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using CAS.Lib.CodeProtect.Properties;
using System;
using System.Diagnostics;

namespace CAS.Lib.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Local trace source to provide all problems with obtaining the license.
  /// </summary>
  internal class LicenseTraceSource: TraceSource
  {
    #region internal
    /// <summary>
    /// <see cref="TraceEventType.Verbose"/> trace message.
    /// </summary>
    /// <param name="id">User identifier for the message</param>
    /// <param name="message">message that we want to trace</param>
    internal static void TraceVerbose( int id, string message )
    {
      PrivateTrace( TraceEventType.Verbose, id, message );
    }
    /// <summary>
    /// Information trace message.
    /// </summary>
    /// <param name="id">User identifier for the message</param>
    /// <param name="message">message that we want to trace</param>
    internal static void TraceInformation( int id, string message )
    {
      PrivateTrace( TraceEventType.Information, id, message );
    }
    /// <summary>
    /// Warning trace message
    /// </summary>
    /// <param name="id">user identifier for the message</param>
    /// <param name="message">message that we want to trace</param>
    internal static void TraceWarning( int id, string message )
    {
      PrivateTrace( TraceEventType.Warning, id, message );
    }
    /// <summary>
    /// Error trace message
    /// </summary>
    /// <param name="id">User identifier for the message</param>
    /// <param name="message">Message that we want to trace</param>
    internal static void TraceError( int id, string message )
    {
      PrivateTrace( TraceEventType.Error, id, message );
    }
    /// <summary>
    /// Gets instance of this class created to trace license issues.
    /// </summary>
    /// <value>Instance of this class <see cref="LicenseTraceSource"/>.</value>
    internal static LicenseTraceSource This { get { return m_Source; } }
    #endregion

    #region private
    private LicenseTraceSource()
      : base( Settings.Default.TraceSourceName ) { }
    private static void PrivateTrace( TraceEventType type, int id, string message )
    {
      try
      {
        m_Source.TraceEvent( type, id, message );
      }
      catch ( Exception ) { }
    }
    private static LicenseTraceSource m_Source = new LicenseTraceSource();
    #endregion
  }
}
