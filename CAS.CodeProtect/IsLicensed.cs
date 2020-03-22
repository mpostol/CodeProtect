//<summary>
//  Title   : Helper class getting access to a license for the particular type.
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2008, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@UAOOI.eu
//  http://www.UAOOI.eu
//</summary>

using UAOOI.CodeProtect.EnvironmentAccess;
using UAOOI.CodeProtect.LicenseDsc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace UAOOI.CodeProtect
{
  /// <summary>
  /// Helper interface getting access to the basic information about the license.
  /// This interface might be useful because class IsLicensed&lt;type&gt; is generic
  /// </summary>
  public interface IIsLicensed
  {
    /// <summary>
    /// Gets or sets the volume constrain.
    /// </summary>
    /// <value>The volume. Null if not valid license found.</value>
    int? Volume { get; }
    /// <summary>
    /// Gets or sets the warning.
    /// </summary>
    /// <value>The warning.</value>
    List<string> Warning { get; }
    /// <summary>
    /// Gets or sets the runtime constrain. Null if not valid license found.
    /// </summary>
    /// <value>The run time <see cref="TimeSpan"/>.</value>
    TimeSpan? RunTime { get; }
    /// <summary>
    /// Gets or sets a value indicating whether the type is licensed.
    /// </summary>
    /// <value><c>true</c> if licensed; otherwise, <c>false</c>.</value>
    bool Licensed { get; }
  }
  /// <summary>
  /// Helper class getting access to a license for the particular type.
  /// </summary>
  /// <typeparam name="type">The type of the licensed type. The type has to be sealed class.</typeparam>
  public abstract class IsLicensed<type> : IIsLicensed where type : class
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IsLicensed{type}"/> class.
    /// </summary>
    public IsLicensed()
      : this(null, null)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="IsLicensed{type}"/> class. It calls <see cref="TraceFailureReason"/>, 
    /// <see cref="TraceNoLicenseFile"/>, <see cref="TraceCurrentLicense"/> to provide more information about the validation process.
    /// </summary>
    /// <param name="defaultVolume">The default volume assigned if license does not have valid constrain for the type.</param>
    /// <param name="defaultRunTime">The default runtime assigned if license does not have valid constrain for the type.</param>
    /// <remarks>
    /// Depending on the situation the constructor calls the virtual methods in the following order:
    /// <list type="bullet">
    /// <item><see cref="TraceNoLicenseFile"/> - is called only if license file cannot be opened.</item>
    /// <item><see cref="TraceFailureReason"/> - is called only if license  is not valid.</item>
    /// <item><see cref="TraceCurrentLicense"/> - is called if the license is valid.</item>
    /// <item><see cref="TraceWarning"/> - is called only if license is valid and warning is provided.</item>
    /// </list>
    /// </remarks>
    public IsLicensed(int? defaultVolume, TimeSpan? defaultRunTime)
    {
      if (!typeof(type).IsSealed)
        throw new ApplicationException($"The IsLicensed<type> generic class is valid only for types that are sealed, but the {typeof(type).Name} is not");
      Volume = defaultVolume;
      RunTime = defaultRunTime;
      Licensed = false;
      License lic = null;
      LicenseManager.IsValid(typeof(type), this, out lic);
      LicenseFile m_license = lic as LicenseFile;
      if (m_license == null)
      {
        TraceNoLicenseFile(CodeProtectLP.OpeningError);
        return;
      }
      using (m_license)
      {
        if (!String.IsNullOrEmpty(m_license.FailureReason))
        {
          TraceFailureReason(m_license.FailureReason);
          return;
        }
        Licensed = true;
        Volume = m_license.VolumeConstrain >= 0 ? m_license.VolumeConstrain : int.MaxValue;
        RunTime = m_license.RunTimeConstrain >= 0 ? TimeSpan.FromHours(m_license.RunTimeConstrain) : TimeSpan.MaxValue;
        TraceCurrentLicense(m_license);
        if (m_license.Warnings.Count > 0)
        {
          TraceWarning(m_license.Warnings);
          Warning = m_license.Warnings;
        }
      }
    }
    /// <summary>
    /// Gets or sets the volume constrain.
    /// </summary>
    /// <value>The volume. Null if not valid license found.</value>
    public int? Volume { get; private set; }
    /// <summary>
    /// Gets or sets the warning.
    /// </summary>
    /// <value>The warning.</value>
    public List<string> Warning { get; private set; }
    /// <summary>
    /// Gets or sets the runtime constrain. Null if not valid license found.
    /// </summary>
    /// <value>The run time <see cref="TimeSpan"/>.</value>
    public TimeSpan? RunTime { get; private set; }
    /// <summary>
    /// Gets or sets a value indicating whether the type is licensed.
    /// </summary>
    /// <value><c>true</c> if licensed; otherwise, <c>false</c>.</value>
    public bool Licensed { get; private set; }
    /// <summary>
    /// It is called by the constructor at the end of the license validation process if a proper license file can be opened 
    /// but the license is invalid for the provided <typeparamref name="type"/>. 
    /// By default it traces the information about the problem to the <see cref="LicenseTraceSource"/> as warning message.
    /// If implemented by the derived class 
    /// allows caller to write license failure reason to a log.
    /// </summary>
    /// <param name="reason">The reason of the license failure.</param>
    protected virtual void TraceFailureReason(string reason)
    {
      string msg = "Cannot activate program feature because: {0}.";
      LicenseTraceSource.TraceWarning(0, String.Format(msg, reason));
    }
    /// <summary>
    /// It is called by the constructor if a proper license file cannot be opened. 
    /// By default it traces the information about the problem to the <see cref="LicenseTraceSource"/> as error message.
    /// If implemented by the derived class allows caller to write license failure reason to a log.
    /// </summary>
    protected virtual void TraceNoLicenseFile(string reason)
    {
      string msg = "No license has been found, because: {0}.";
      LicenseTraceSource.TraceError(0, String.Format(msg, reason));
    }
    /// <summary>
    /// It is called by the constructor if a warning appears after reading a license
    /// By default it traces the information about the warnings to the <see cref="LicenseTraceSource"/> as information message.
    /// If implemented by the derived class allows caller to show warning or write it to a log.
    /// </summary>
    protected virtual void TraceWarning(List<string> warning)
    {
      string msg = "After loading a license the following warning appeared: {0}.";
      LicenseTraceSource.TraceInformation(0, String.Format(msg, string.Concat(warning.ToArray())));
    }
    /// <summary>
    /// It is called by the constructor at the end of the license validation process if a proper license file can be opened. 
    /// By default it traces the information about the license to the <see cref="LicenseTraceSource"/> as verbose message.
    /// If implemented by the derived class the current license can be used to get more information from it.
    /// </summary>
    /// <param name="license">The current license for temporal use. The current license is disposed just after returning from this method
    /// </param>
    /// <remarks>
    /// Because the current license is disposed in the base class just after returning from this method, it must not be
    /// assigned locally to keep it for future use. To keep it for future use the derived class must make a local copy.
    /// </remarks>
    protected virtual void TraceCurrentLicense(LicenseFile license)
    {
      string format = "Obtained valid license for {0}, runtime={1}, volume={2}";
      string msg = string.Format(format, typeof(type).FullName, license.RunTimeConstrain, license.VolumeConstrain);
      LicenseTraceSource.TraceVerbose(0, msg);
    }
  }
}
