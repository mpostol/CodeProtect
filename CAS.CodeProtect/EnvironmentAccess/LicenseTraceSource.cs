//___________________________________________________________________________________
//
//  Copyright (C) 2020, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________


using System;
using System.Diagnostics;

namespace UAOOI.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Local trace source to provide all problems with obtaining the license.
  /// </summary>
  internal class LicenseTraceSource : TraceSource
  {

    #region internal
    /// <summary>
    /// <see cref="TraceEventType.Verbose"/> trace message.
    /// </summary>
    /// <param name="id">User identifier for the message</param>
    /// <param name="message">message that we want to trace</param>
    internal static void TraceVerbose(int id, string message)
    {
      PrivateTrace(TraceEventType.Verbose, id, message);
    }
    /// <summary>
    /// Information trace message.
    /// </summary>
    /// <param name="id">User identifier for the message</param>
    /// <param name="message">message that we want to trace</param>
    internal static void TraceInformation(int id, string message)
    {
      PrivateTrace(TraceEventType.Information, id, message);
    }
    /// <summary>
    /// Warning trace message
    /// </summary>
    /// <param name="id">user identifier for the message</param>
    /// <param name="message">message that we want to trace</param>
    internal static void TraceWarning(int id, string message)
    {
      PrivateTrace(TraceEventType.Warning, id, message);
    }
    /// <summary>
    /// Error trace message
    /// </summary>
    /// <param name="id">User identifier for the message</param>
    /// <param name="message">Message that we want to trace</param>
    internal static void TraceError(int id, string message)
    {
      PrivateTrace(TraceEventType.Error, id, message);
    }
    /// <summary>
    /// Gets instance of this class created to trace license issues.
    /// </summary>
    /// <value>Instance of this class <see cref="LicenseTraceSource"/>.</value>
    internal static LicenseTraceSource This { get { return m_Source.Value; } }
    #endregion

    #region private
    private LicenseTraceSource()
      : base(Properties.Resources.TraceSourceName) { }
    private static void PrivateTrace(TraceEventType type, int id, string message)
    {
      try
      {
        m_Source.Value.TraceEvent(type, id, message);
      }
      catch (Exception) { }
    }
    private static Lazy<LicenseTraceSource> m_Source = new Lazy<LicenseTraceSource>(() => new LicenseTraceSource());
    #endregion
  }
}
