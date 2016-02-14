//<summary>
//  Title   : A control to support licenses management
//  System  : Microsoft Visual C# .NET 2005
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//  History :
//    20080508: mzbrzezny namespace is changed to CAS.Lib.ControlLibrary
//    MPostol - 19-03-2007: created 
//
//  Copyright (C)2006, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto:techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CAS.Lib.CodeProtect.EnvironmentAccess;
using CAS.Lib.CodeProtect.LicenseDsc;
using CAS.Lib.CodeProtect.Properties;

namespace CAS.Lib.CodeProtect.Controls
{
  /// <summary>
  /// A control to support licenses management
  /// </summary>
  public partial class Licences: UserControl
  {
    #region public
    #region creator
    /// <summary>
    /// Initializes a new instance of the <see cref="Licences"/> class.
    /// </summary>
    public Licences()
    {
      InitializeComponent();
    }
    #endregion
    /// <summary>
    /// Gets the description of the license.
    /// </summary>
    /// <returns>The license decription with basic information.</returns>
    public string GetLicenseMessageRequest()
    {
      try
      {
        LicenseFile m_License = null;
        using ( m_License = LicenseFile.LoadFile
           ( FileNames.CreateLicenseFileStream( FileMode.Open, FileAccess.Read, FileShare.Read ), null, false ) )
        {
          StringBuilder sb = new StringBuilder();
          sb.AppendLine( Resources.License_request_message );
          sb.AppendLine( GetMainLicenseInformation( m_License ) );
          return sb.ToString();
        }
      }
      catch ( Exception ex )
      {
        MessageBox.Show( ex.Message );
        return "";
      }
    }

    /// <summary>
    /// Gets the information about the license
    /// </summary>
    /// <returns></returns>
    public string GetLicenseInfo()
    {
      try
      {
        LicenseFile m_License = null;
        using ( m_License = LicenseFile.LoadFile
           ( FileNames.CreateLicenseFileStream( FileMode.Open, FileAccess.Read, FileShare.Read ), null, false ) )
        {
          return m_License.Product.FullName;
        }
      }
      catch ( Exception ex )
      {
        MessageBox.Show( ex.Message );
        return "";
      }
    }

    #endregion

    #region private
    private static string GetMainLicenseInformation( LicenseFile clic )
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine( "This license is issued for:" + clic.Product.FullName );
      sb.AppendLine( "\tDescription:" + clic.Product.Description );
      sb.AppendLine( "This product is licensed for:" );
      sb.AppendLine( "\tUser name:\t" + clic.User.Name );
      sb.AppendLine( "\tUser organization:\t" + clic.User.Organization );
      sb.AppendLine( "\tUser email:\t" + clic.User.Email );
      sb.AppendLine( "License general information:" );
      sb.AppendLine( "\tHardware key:\t" + clic.HardwareKeyTokenBasedOnDevice );
      sb.AppendLine( "\tLicense key:\t" + clic.LicenseKey );
      return sb.ToString();
    }
    private void OpenLicense()
    {
      try
      {
        LicenseFile m_License = null;
        using ( m_License = LicenseFile.LoadFile
           ( FileNames.CreateLicenseFileStream( FileMode.Open, FileAccess.Read, FileShare.Read ), null, false ) )
        {
          StringBuilder sb = new StringBuilder();
          sb.AppendLine( GetMainLicenseInformation( m_License ) );
          sb.AppendLine();
          sb.AppendLine( "Constrains attached to this license:" );
          foreach ( IConstraint con in ( (LicenseFile)m_License ).Constraints )
            sb.AppendLine( con.ToString() );
          m_textBoxLIcDescriptor.Text = sb.ToString();
        }
      }
      catch ( Exception ex )
      {
        m_textBoxLIcDescriptor.Text = Resources.Tx_LicNoFileErr + " : " + ex.Message;
        return;
      }
    }
    #region event handlers
    private void m_InstallLicBut_Click( object sender, EventArgs e )
    {
      if ( m_OpenFileDialog.ShowDialog() != DialogResult.OK )
        return;
      try 
      { 
        CodeProtect.LicenseDsc.LicenseFile.UpgardeLicense( m_OpenFileDialog.FileName );
        MessageBox.Show( "License upgrade has been successfully installed", "Upgarde license", MessageBoxButtons.OK, MessageBoxIcon.Information );
      }
      catch ( Exception ex )
      {
        string mess = "Instalationlicense failure: ";
        mess = LicenseFileException.TraceInnerExceptions( ex, mess );
        MessageBox.Show( mess, Resources.CaptionUpgardeLicense, MessageBoxButtons.OK, MessageBoxIcon.Error );
      }
    }
    private void button_export_info_Click( object sender, EventArgs e )
    {
      try
      {
        if ( m_saveFileDialog.ShowDialog() == DialogResult.OK )
        {
          FileInfo finfo = new FileInfo( m_saveFileDialog.FileName );
          if ( finfo.Exists )
            finfo.Delete();
          using ( StreamWriter file = new StreamWriter( m_saveFileDialog.FileName ) )
          {
            file.Write( m_textBoxLIcDescriptor.Text );
          }
          Process.Start( m_saveFileDialog.FileName );
        }
      }
      catch ( Exception ex )
      {
        MessageBox.Show( ex.Message );
      }
    }
    private void Licences_Load( object sender, EventArgs e )
    {
      OpenLicense();
    }
    #endregion event handlers
    #endregion private
  }
}
