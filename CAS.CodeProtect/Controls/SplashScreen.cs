//<summary>
//  Title   : SplashScreen: UA Model Designer
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2008, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CAS.Lib.CodeProtect.Properties;

namespace CAS.Lib.CodeProtect.Controls
{
  /// <summary>
  /// SplashScreen to provide information about events while starting.
  /// </summary>
  public partial class SplashScreen: Form
  {
    #region public
    /// <summary>
    /// This class trace to a text box on on the <see cref="SplashScreen"/> failure reason and warning.
    /// </summary>
    /// <typeparam name="type">The type of the licensed type that must be sealed.</typeparam>
    public class LogedIsLicensed<type>: IsLicensed<type> where type: class
    {
      /// <summary>
      /// It is called by the constructor at the end of the license validation process if a proper license file can be opened
      /// but the license is invalid for the provided <typeparamref name="type"/>.
      /// By default it traces the information about the problem to the <see cref="CAS.Lib.CodeProtect.EnvironmentAccess.LicenseTraceSource"/> as warning message.
      /// If implemented by the derived class
      /// allows caller to write license failure reason to a log.
      /// </summary>
      /// <param name="reason">The reason of the license failure.</param>
      protected override void TraceFailureReason( string reason )
      {
        SplashScreen.AppendTextIfInstantiated( reason, false );
        base.TraceFailureReason( reason );
      }
      /// <summary>
      /// It is called by the constructor if a warning appears after reading a license
      /// By default it traces the information about the warnings to the <see cref="CAS.Lib.CodeProtect.EnvironmentAccess.LicenseTraceSource"/> as information message.
      /// If implemented by the derived class allows caller to show warning or write it to a log.
      /// </summary>
      /// <param name="warning"></param>
      protected override void TraceWarning( System.Collections.Generic.List<string> warning )
      {
        StringBuilder sb = new StringBuilder();
        foreach ( var item in warning )
          sb.AppendLine( item );
        SplashScreen.AppendTextIfInstantiated( sb.ToString(), true );
        base.TraceWarning( warning );
      }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SplashScreen"/> class.
    /// </summary>
    /// <param name="showSplashScreen">If set to <c>true</c> <see cref="Show"/> method show splash screen, othervise it is hiden . .</param>
    /// <param name="maintenancePeriodWarning">If set to <c>true</c> maintenance period warning has alraedy been issed.</param>
    public SplashScreen( bool showSplashScreen, bool maintenancePeriodWarning )
      : this()
    {
      ShowSplashScreen = showSplashScreen;
      MaintenancePeriodWarning = maintenancePeriodWarning;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SplashScreen"/> class.
    /// </summary>
    public SplashScreen()
    {
      InitializeComponent();
      this.TopLevel = true;
      this.m_BuyNowButton.Visible = false;
      this.m_StartAsmdButton.Visible = false;
      this.m_ShowItCheckBox.Checked = ShowSplashScreen;
      m_Instance = this;
    }
    /// <summary>
    /// Appends the text to message box on the UI and actvates wait loop to keep
    /// the form on the screen until the start button is pressed. Ignored if the UI is not instantiated or disposing.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="forceKeyPress">if set to <c>true</c> force key press before closing this form. 
    /// It is ignored if the <paramref name="message"/> is null or empty</param>
    public static void AppendTextIfInstantiated( string message, bool forceKeyPress )
    {
      if ( m_Instance == null )
        return;
      m_Instance.AppendText( message, forceKeyPress );
    }
    /// <summary>
    /// Appends the text to message box on the UI and actvates wait loop to keep
    /// the form on the screen until the start button is pressed.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="forceKeyPress">if set to <c>true</c> force key press before closing this form. 
    /// It is ignored if the <paramref name="message"/> is null or empty</param>
    public void AppendText( string message, bool forceKeyPress )
    {
      if ( String.IsNullOrEmpty( message ) )
        return;
      this.m_LicenseInfo.AppendText( string.Format( m_NewLine, message ) );
      this.Refresh();
      if ( forceKeyPress )
        CanBeClosed = false;
    }
    /// <summary>
    /// Actvates the BuyNow button and wait loop to keep
    /// the form on the screen until the start button is pressed.
    /// </summary>
    public void ActivateBuyNow()
    {
      CanBeClosed = false;
      m_StartAsmdButton.Text = StartButtonLabel();
      m_BuyNowButton.Visible = true;
      wholeCounterTime = m_Delay;
      ShowSplashScreen = true;
    }
    /// <summary>
    /// Closes the splash screen.
    /// </summary>
    public void CloseSplashScreen()
    {
      if ( wholeCounterTime > 0 )
      {
        m_Timer.Start();
        m_Timer.Enabled = true;
      }
      else
      {
        m_StartAsmdButton.Enabled = true;
        m_StartAsmdButton.Text = Resources.SplashScreenStartASMDButtonText;
      }
      while ( !CanBeClosed ){
        Thread.Sleep(100);
        Application.DoEvents();}
      this.Close();
    }
    /// <summary>
    /// Displays the control to the user.
    /// </summary>
    /// <PermissionSet>
    /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
    /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    /// </PermissionSet>
    public new void Show()
    {
      if ( ShowSplashScreen )
      {
        base.Show();
        Refresh();
      }
    }
    /// <summary>
    /// Argument (<see cref="System.EventArgs"/>) providing a bool value.
    /// </summary>
    public class BoolEventArg: EventArgs
    {
      /// <summary>
      /// Gets or sets a value of this <see cref="BoolEventArg"/> class.
      /// </summary>
      /// <value><c>true</c> if value; otherwise, <c>false</c>.</value>
      public bool Value { get; set; }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show splash screen at startup.
    /// </summary>
    /// <value><c>true</c> if [show splash screen]; otherwise, <c>false</c>.</value>
    public bool ShowSplashScreen
    {
      get { return m_ShowSplashScreen; }
      set
      {
        if ( ( value != m_ShowSplashScreen ) && ( OnShowSplashScreenChanged != null ) )
          OnShowSplashScreenChanged( this, new BoolEventArg() { Value = value } );
        m_ShowSplashScreen = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether maintenance period warning has already be signaled.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [maintenance period warning]; otherwise, <c>false</c>.
    /// </value>
    public bool MaintenancePeriodWarning
    {
      get { return m_MaintenancePeriodWarning; }
      set
      {
        if ( ( value != m_MaintenancePeriodWarning ) && ( OnMaintenancePeriodWarning != null ) )
          OnMaintenancePeriodWarning( this, new BoolEventArg() { Value = value } );
        m_ShowSplashScreen = value;
      }
    }
    /// <summary>
    /// Occurs when show splash screen at startup check box is changed. It could be used to preseve the value
    /// </summary>
    public event EventHandler<BoolEventArg> OnShowSplashScreenChanged;
    /// <summary>
    /// Occurs when maintenance period is detected and the value of the MaintenancePeriodWarning is changed.
    /// </summary>
    public event EventHandler<BoolEventArg> OnMaintenancePeriodWarning;
    #endregion internal

    #region private
    private bool m_ShowSplashScreen = true;
    private bool m_MaintenancePeriodWarning = false;
    /// <summary>
    /// Disposes of the resources (other than memory) used by the <see cref="T:System.Windows.Forms.Form"/>.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose( bool disposing )
    {
      if ( disposing && ( components != null ) )
      {
        components.Dispose();
      }
      m_Instance = null;
      base.Dispose( disposing );
    }
    private static SplashScreen m_Instance;
    private short m_Delay = 5;
    private const string m_NewLine = "\r\n{0}\r\n";
    private readonly string m_StartButtonFormat = Resources.SplashScreenStartButtonFormat;
    private int wholeCounterTime = 0;
    private bool m_CanBeClosed = true;
    private bool CanBeClosed
    {
      get { return m_CanBeClosed; }
      set
      {
        m_CanBeClosed = value;
        if ( !value )
        {
          m_StartAsmdButton.Visible = true;
          base.Show();
        }
      }
    }
    /// <summary>
    /// Handles the Click event of the buyNowButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void m_Timer_Tick( object sender, EventArgs e )
    {
      if ( wholeCounterTime == 0 )
      {
        m_StartAsmdButton.Enabled = true;
        m_StartAsmdButton.Text = Resources.SplashScreenStartASMDButtonText;
        m_Timer.Enabled = false;
      }
      else
      {
        m_StartAsmdButton.Text = StartButtonLabel();
        m_StartAsmdButton.Refresh();
        wholeCounterTime--;
      };
    }
    private string StartButtonLabel()
    {
      return String.Format( m_StartButtonFormat, wholeCounterTime.ToString() );
    }
    private void startAsmdButton_Click( object sender, EventArgs e )
    {
      CanBeClosed = true;
    }
    private void buyNowButton_Click( object sender, EventArgs e )
    {
      try
      {
        Process.Start( Resources.SplashScreenStoreUrl );
      }
      catch ( Win32Exception ex )
      {
        MessageBox.Show( String.Format( Resources.SplashScreenDefaultAppMissing, Resources.SplashScreenStoreUrl, ex.Message ) );
      }
    }
    private void m_ShowItCheckBox_CheckedChanged( object sender, EventArgs e )
    {
      ShowSplashScreen = m_ShowItCheckBox.Checked;
    }
    private void SplashScreen_Load( object sender, EventArgs e )
    {
      AppendText( string.Format( Resources.SplashScreenWaitMessage, Assembly.GetEntryAssembly().GetName().Name ), false );
      AppendText( m_MaintenanceControlComponent.Warning, false );
      if ( m_MaintenanceControlComponent.Licensed && !MaintenancePeriodWarning )
        ShowSplashScreen = true;
      MaintenancePeriodWarning = m_MaintenanceControlComponent.Licensed;
    }
    #endregion private
  }
}
