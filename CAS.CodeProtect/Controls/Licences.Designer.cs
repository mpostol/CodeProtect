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

namespace CAS.Lib.CodeProtect.Controls
{
  partial class Licences
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if ( disposing && ( components != null ) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }
    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.Label m_LicenseDescriptorLabel;
      this.m_textBoxLIcDescriptor = new System.Windows.Forms.TextBox();
      this.m_InstallLicBut = new System.Windows.Forms.Button();
      this.m_OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.button_export_info = new System.Windows.Forms.Button();
      this.m_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      m_LicenseDescriptorLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // m_LicenseDescriptorLabel
      // 
      m_LicenseDescriptorLabel.AutoSize = true;
      m_LicenseDescriptorLabel.Location = new System.Drawing.Point( 0, 0 );
      m_LicenseDescriptorLabel.Name = "m_LicenseDescriptorLabel";
      m_LicenseDescriptorLabel.Size = new System.Drawing.Size( 77, 13 );
      m_LicenseDescriptorLabel.TabIndex = 15;
      m_LicenseDescriptorLabel.Text = "License details";
      m_LicenseDescriptorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // m_textBoxLIcDescriptor
      // 
      this.m_textBoxLIcDescriptor.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                  | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.m_textBoxLIcDescriptor.Location = new System.Drawing.Point( 3, 16 );
      this.m_textBoxLIcDescriptor.Multiline = true;
      this.m_textBoxLIcDescriptor.Name = "m_textBoxLIcDescriptor";
      this.m_textBoxLIcDescriptor.ReadOnly = true;
      this.m_textBoxLIcDescriptor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.m_textBoxLIcDescriptor.Size = new System.Drawing.Size( 377, 248 );
      this.m_textBoxLIcDescriptor.TabIndex = 14;
      this.m_textBoxLIcDescriptor.WordWrap = false;
      // 
      // m_InstallLicBut
      // 
      this.m_InstallLicBut.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.m_InstallLicBut.Location = new System.Drawing.Point( 105, 270 );
      this.m_InstallLicBut.Margin = new System.Windows.Forms.Padding( 2 );
      this.m_InstallLicBut.Name = "m_InstallLicBut";
      this.m_InstallLicBut.Size = new System.Drawing.Size( 273, 34 );
      this.m_InstallLicBut.TabIndex = 11;
      this.m_InstallLicBut.Text = "Install new license";
      this.m_InstallLicBut.UseVisualStyleBackColor = true;
      this.m_InstallLicBut.Click += new System.EventHandler( this.m_InstallLicBut_Click );
      // 
      // m_OpenFileDialog
      // 
      this.m_OpenFileDialog.DefaultExt = "lic";
      this.m_OpenFileDialog.Filter = "Licenses {*.lic} |*.lic";
      this.m_OpenFileDialog.Title = "Open license file";
      // 
      // button_export_info
      // 
      this.button_export_info.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
      this.button_export_info.Location = new System.Drawing.Point( 3, 270 );
      this.button_export_info.Name = "button_export_info";
      this.button_export_info.Size = new System.Drawing.Size( 97, 34 );
      this.button_export_info.TabIndex = 16;
      this.button_export_info.Text = "Export license information ...";
      this.button_export_info.UseVisualStyleBackColor = true;
      this.button_export_info.Click += new System.EventHandler( this.button_export_info_Click );
      // 
      // m_saveFileDialog
      // 
      this.m_saveFileDialog.Filter = "License information as text file {*.txt} |*.txt";
      this.m_saveFileDialog.Title = "Select file to save license information";
      // 
      // Licences
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add( this.button_export_info );
      this.Controls.Add( m_LicenseDescriptorLabel );
      this.Controls.Add( this.m_textBoxLIcDescriptor );
      this.Controls.Add( this.m_InstallLicBut );
      this.Name = "Licences";
      this.Size = new System.Drawing.Size( 380, 319 );
      this.Load += new System.EventHandler( this.Licences_Load );
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button m_InstallLicBut;
    private System.Windows.Forms.OpenFileDialog m_OpenFileDialog;
    private System.Windows.Forms.TextBox m_textBoxLIcDescriptor;
    private System.Windows.Forms.Button button_export_info;
    private System.Windows.Forms.SaveFileDialog m_saveFileDialog;
  }
}
