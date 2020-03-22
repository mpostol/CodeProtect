//<summary>
//  Title   : Maintenance constrain controller
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2009, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@UAOOI.eu
//  http://www.UAOOI.eu
//</summary>

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace UAOOI.CodeProtect
{
  /// <summary>
  /// Component that is responsible for checking the period after which the maintenance package for software will expire.
  /// </summary>
  public partial class MaintenanceControlComponent: Component
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MaintenanceControlComponent"/> class.
    /// </summary>
    public MaintenanceControlComponent()
    {
      InitializeComponent();
      myTraceControl = new MaintenanceControl();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="MaintenanceControlComponent"/> class.
    /// </summary>
    /// <param name="container">The container.</param>
    public MaintenanceControlComponent( IContainer container )
    {
      container.Add( this );
      InitializeComponent();
      myTraceControl = new MaintenanceControl();
    }
    /// <summary>
    /// Gets or sets the warning.
    /// </summary>
    /// <value>The warning.</value>
    public string Warning { get { return myTraceControl.MaintenanceControlWarning; } }
    /// <summary>
    /// Gets a value indicating whether this <see cref="MaintenanceControlComponent"/> is licensed.
    /// </summary>
    /// <value><c>true</c> if licensed; otherwise, <c>false</c>.</value>
    public bool Licensed { get { return myTraceControl.Licensed; } }
    /// <summary>
    /// MaintenanceControl member
    /// </summary>
    private MaintenanceControl myTraceControl;
  }

  /// <summary>
  /// Class that is used to control maintenance
  /// </summary>
  [LicenseProvider( typeof( CodeProtectLP ) )]
  [GuidAttribute( "64FBD005-2D72-476b-8523-E298D5FC2E79" )]
  public sealed class MaintenanceControl: IsLicensed<MaintenanceControl>
  {
    /// <summary>
    /// Gets or sets the warning.
    /// </summary>
    /// <value>The warning.</value>
    public string MaintenanceControlWarning { get; private set; }
    /// <summary>
    /// It is called by the constructor if a warning appears after reading a license
    /// If implemented by the derived class allows caller to show warning or write it to a log.
    /// </summary>
    /// <param name="warningList"></param>
    protected override void TraceWarning( List<string> warningList )
    {
      MaintenanceControlWarning = string.Concat( warningList.ToArray() );
    }
  }
}


