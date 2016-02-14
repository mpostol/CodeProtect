namespace CAS.Lib.CodeProtect.Controls
{
  public partial class SplashScreen
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( SplashScreen ) );
      this.m_PictureBox = new System.Windows.Forms.PictureBox();
      this.m_LicenseInfo = new System.Windows.Forms.RichTextBox();
      this.m_BuyNowButton = new System.Windows.Forms.Button();
      this.m_StartAsmdButton = new System.Windows.Forms.Button();
      this.m_Timer = new System.Windows.Forms.Timer( this.components );
      this.m_ShowItCheckBox = new System.Windows.Forms.CheckBox();
      this.m_MaintenanceControlComponent = new CAS.Lib.CodeProtect.MaintenanceControlComponent( this.components );
      ( (System.ComponentModel.ISupportInitialize)( this.m_PictureBox ) ).BeginInit();
      this.SuspendLayout();
      // 
      // m_PictureBox
      // 
      this.m_PictureBox.Image = global::CAS.Lib.CodeProtect.Properties.Resources.UAASMDInstallbanner;
      this.m_PictureBox.InitialImage = global::CAS.Lib.CodeProtect.Properties.Resources.UAASMDInstallbanner;
      this.m_PictureBox.Location = new System.Drawing.Point( 0, 0 );
      this.m_PictureBox.Name = "m_PictureBox";
      this.m_PictureBox.Size = new System.Drawing.Size( 633, 88 );
      this.m_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.m_PictureBox.TabIndex = 0;
      this.m_PictureBox.TabStop = false;
      // 
      // m_LicenseInfo
      // 
      this.m_LicenseInfo.BackColor = System.Drawing.SystemColors.Window;
      this.m_LicenseInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.m_LicenseInfo.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 238 ) ) );
      this.m_LicenseInfo.ForeColor = System.Drawing.Color.Black;
      this.m_LicenseInfo.Location = new System.Drawing.Point( 12, 94 );
      this.m_LicenseInfo.Name = "m_LicenseInfo";
      this.m_LicenseInfo.ReadOnly = true;
      this.m_LicenseInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
      this.m_LicenseInfo.Size = new System.Drawing.Size( 607, 430 );
      this.m_LicenseInfo.TabIndex = 2;
      this.m_LicenseInfo.Text = "";
      // 
      // m_BuyNowButton
      // 
      this.m_BuyNowButton.Font = new System.Drawing.Font( "Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 238 ) ) );
      this.m_BuyNowButton.ForeColor = System.Drawing.SystemColors.WindowText;
      this.m_BuyNowButton.Location = new System.Drawing.Point( 12, 530 );
      this.m_BuyNowButton.Name = "m_BuyNowButton";
      this.m_BuyNowButton.Size = new System.Drawing.Size( 216, 50 );
      this.m_BuyNowButton.TabIndex = 3;
      this.m_BuyNowButton.Text = "Buy Now !";
      this.m_BuyNowButton.UseVisualStyleBackColor = true;
      this.m_BuyNowButton.Click += new System.EventHandler( this.buyNowButton_Click );
      // 
      // m_StartAsmdButton
      // 
      this.m_StartAsmdButton.Enabled = false;
      this.m_StartAsmdButton.Font = new System.Drawing.Font( "Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 238 ) ) );
      this.m_StartAsmdButton.Location = new System.Drawing.Point( 403, 530 );
      this.m_StartAsmdButton.Name = "m_StartAsmdButton";
      this.m_StartAsmdButton.Size = new System.Drawing.Size( 216, 50 );
      this.m_StartAsmdButton.TabIndex = 4;
      this.m_StartAsmdButton.Text = "Wait";
      this.m_StartAsmdButton.UseVisualStyleBackColor = true;
      this.m_StartAsmdButton.Click += new System.EventHandler( this.startAsmdButton_Click );
      // 
      // m_Timer
      // 
      this.m_Timer.Interval = 1000;
      this.m_Timer.Tick += new System.EventHandler( this.m_Timer_Tick );
      // 
      // m_ShowItCheckBox
      // 
      this.m_ShowItCheckBox.AutoSize = true;
      this.m_ShowItCheckBox.Checked = true;
      this.m_ShowItCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.m_ShowItCheckBox.Location = new System.Drawing.Point( 253, 547 );
      this.m_ShowItCheckBox.Name = "m_ShowItCheckBox";
      this.m_ShowItCheckBox.Size = new System.Drawing.Size( 125, 17 );
      this.m_ShowItCheckBox.TabIndex = 5;
      this.m_ShowItCheckBox.Text = "Show this on startup.";
      this.m_ShowItCheckBox.UseVisualStyleBackColor = true;
      this.m_ShowItCheckBox.CheckedChanged += new System.EventHandler( this.m_ShowItCheckBox_CheckedChanged );
      // 
      // SplashScreen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.ClientSize = new System.Drawing.Size( 631, 592 );
      this.ControlBox = false;
      this.Controls.Add( this.m_ShowItCheckBox );
      this.Controls.Add( this.m_StartAsmdButton );
      this.Controls.Add( this.m_BuyNowButton );
      this.Controls.Add( this.m_LicenseInfo );
      this.Controls.Add( this.m_PictureBox );
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
      this.Name = "SplashScreen";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.TopMost = true;
      this.Load += new System.EventHandler( this.SplashScreen_Load );
      ( (System.ComponentModel.ISupportInitialize)( this.m_PictureBox ) ).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox m_PictureBox;
    private CAS.Lib.CodeProtect.MaintenanceControlComponent m_MaintenanceControlComponent;
    private System.Windows.Forms.RichTextBox m_LicenseInfo;
    private System.Windows.Forms.Button m_BuyNowButton;
    private System.Windows.Forms.Button m_StartAsmdButton;
    private System.Windows.Forms.Timer m_Timer;
    private System.Windows.Forms.CheckBox m_ShowItCheckBox;

  }
}