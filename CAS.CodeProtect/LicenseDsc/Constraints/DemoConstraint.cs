//<summary>
//  Title   : Demo Constraint
//  System  : Microsoft Visual C# .NET 
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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;

namespace UAOOI.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='DemoConstraint'/> constrains the user
  /// to a given time period or duration.  It supports start and end date that the
  /// license will expire.  It also support a duration, to constrain the license to
  /// a number of days.  The constraint contains a purchase URL, Conditions, and
  /// info URL so they may obtain a registered license, the conditions of the
  /// constraint and more information.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class DemoConstraint: AbstractConstraint
  {
    #region private
    private DateTime start = new DateTime();
    private DateTime end = new DateTime();
    private int maxDuration = -1;
    private string infoUrl = String.Empty;
    private string purchaseUrl = String.Empty;
    private string conditions = String.Empty;
    private DateTime dateLastAccessed = DateTime.Now;
    /// <summary>
    /// This is called whenever a license failure occurs to build the Failure
    /// Reason string. It will take the passed in string and append the conditions,
    /// Purchase URL, and Information URL if they are defined.  The returned string
    /// is the complete failure reason for this constraint.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the failure reason for this constraint.
    /// </returns>
    private string BuildErrorString( string errorText )
    {
      StringBuilder errStr = new StringBuilder();
      errStr.Append( errorText );
      if ( this.Condition != String.Empty || this.InfoURL != String.Empty || this.PurchaseURL != String.Empty )
        errStr.Append( "\n" );
      if ( this.Condition != String.Empty )
      {
        errStr.Append( "Condition: " + this.Condition );
        errStr.Append( "\n" );
      }
      if ( this.InfoURL != String.Empty )
      {
        errStr.Append( "Info URL: " + this.InfoURL );
        errStr.Append( "\n" );
      }
      if ( this.PurchaseURL != String.Empty )
        errStr.Append( "Purchase URL: " + this.PurchaseURL );
      return errStr.ToString();
    }
    #endregion
    #region public creators
    /// <summary>
    /// This is the constructor for the <c>DemoConstraint</c>.  The constructor
    /// is used to create the object without a valid license attached it to.
    /// </summary>
    public DemoConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>DemoConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public DemoConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Demo Constraint";
      base.Description = "This DemoConstraint constrains the user to a given time period ";
      base.Description += "or duration.  It supports start and end date that the license ";
      base.Description += "will expire.  It also support a duration, to constrain the ";
      base.Description += "license to.";
    }
    #endregion
    #region AbstractConstraint implementation
    /// <summary>
    /// Prepares the text information about constraint.
    /// </summary>
    /// <param name="cStr">The stringBuilder class where information should be stored</param>
    /// <param name="additionalInformation">The additional information.</param>
    /// <param name="depth">The depth (Indentation  level).</param>
    protected internal override void PrepareTextInformationAboutConstraint( StringBuilder cStr, string additionalInformation, int depth )
    {
      StringBuilder sb_addtionalInfo = new StringBuilder();
      DoIndent( sb_addtionalInfo, depth );
      sb_addtionalInfo.AppendLine( "Condition:\t\t" + Condition );
      DoIndent( sb_addtionalInfo, depth );
      sb_addtionalInfo.AppendLine( "Start Date:\t" + StartDate.ToShortDateString() );
      DoIndent( sb_addtionalInfo, depth );
      sb_addtionalInfo.AppendLine( "End Date:\t" + EndDate.ToShortDateString() );
      DoIndent( sb_addtionalInfo, depth );
      sb_addtionalInfo.AppendLine( "Duration:\t\t" + Duration.ToString() );
      DoIndent( sb_addtionalInfo, depth );
      sb_addtionalInfo.AppendLine( "Info URL:\t\t" + InfoURL );
      DoIndent( sb_addtionalInfo, depth );
      sb_addtionalInfo.AppendLine( "Purchase URL:\t" + PurchaseURL );
      base.PrepareTextInformationAboutConstraint( cStr, sb_addtionalInfo.ToString(), depth );
    }
    /// <summary>
    /// This verifies the license meets its desired validation criteria.  This
    /// includes the following if defined:
    /// <list>
    /// <item>Verify the license is after the start date</item>
    /// <item>Verify the license is before the end data</item>
    /// <item>The maximum number of days has not been exceeded</item>
    /// </list>
    /// If any of these is exceeded then the license failure reason is set to
    /// the reason for the failure and this function returns false.  Otherwise
    /// the license is valid and the method returns true.
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria.  Otherwise
    /// <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to the details of the
    /// failure.  If the Conditions, Purchase URL, or the Information URL is defined
    /// then they will also be added to the failure reason.
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      if ( ( this.Duration > 0 ) && ( this.License.FirstUsesDate.AddDays( this.Duration ) < DateTime.Now ) )
      {
        AddFailureReasonText( "The license has expired." );
        return false;
      }
      if ( this.StartDate.Ticks > 0 && this.StartDate.CompareTo( DateTime.Now ) > 0 )
      {
        AddFailureReasonText
          ( "The license has not been activated yet. It will become active on " + StartDate.ToString( "F" ) );
        return false;
      }
      if ( this.EndDate.Ticks > 0 && this.EndDate.CompareTo( DateTime.Now ) < 0 )
      {
        AddFailureReasonText( "The license has expired." );
        return false;
      }
      this.dateLastAccessed = DateTime.Now;
      return true;
    }
    /// <summary>
    /// This creates a <c>DemoConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>DemoConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode )
    {
      base.FromXml( itemsNode );
      XmlNode startDateTextNode = itemsNode.SelectSingleNode( "StartDate/text()" );
      XmlNode endDateTextNode = itemsNode.SelectSingleNode( "EndDate/text()" );
      XmlNode durationTextNode = itemsNode.SelectSingleNode( "Duration/text()" );
      XmlNode infoURLTextNode = itemsNode.SelectSingleNode( "InfoUrl/text()" );
      XmlNode purchaseURLTextNode = itemsNode.SelectSingleNode( "PurchaseUrl/text()" );
      XmlNode conditionTextNode = itemsNode.SelectSingleNode( "Condition/text()" );
      XmlNode dateAccessedTextNode = itemsNode.SelectSingleNode( "DateLastAccessed/text()" );
      if ( startDateTextNode != null )
        this.StartDate = DateTime.Parse( startDateTextNode.Value, culture, DateTimeStyles.NoCurrentDateDefault );
      if ( endDateTextNode != null )
        this.EndDate = DateTime.Parse( endDateTextNode.Value, culture, DateTimeStyles.NoCurrentDateDefault );
      if ( durationTextNode != null )
        this.Duration = Convert.ToInt32( durationTextNode.Value );
      if ( infoURLTextNode != null )
        this.InfoURL = infoURLTextNode.Value;
      if ( purchaseURLTextNode != null )
        this.PurchaseURL = purchaseURLTextNode.Value;
      if ( conditionTextNode != null )
        this.Condition = conditionTextNode.Value;
      if ( dateAccessedTextNode != null )
        this.dateLastAccessed = DateTime.Parse( dateAccessedTextNode.Value, culture, DateTimeStyles.NoCurrentDateDefault );
    }
    /// <summary>
    /// Converts this <c>DemoConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the DemoConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter )
    {
      xmlWriter.WriteElementString( "Duration", Convert.ToString( this.Duration ) );
      xmlWriter.WriteElementString( "InfoUrl", InfoURL );
      xmlWriter.WriteElementString( "PurchaseUrl", PurchaseURL );
      xmlWriter.WriteElementString( "Condition", Condition );
      if ( StartDate.Ticks != 0 )
        xmlWriter.WriteElementString( "StartDate", StartDate.ToString( culture ) );
      else
        xmlWriter.WriteElementString( "StartDate", String.Empty );
      if ( EndDate.Ticks != 0 )
        xmlWriter.WriteElementString( "EndDate", EndDate.ToString( culture ) );
      else
        xmlWriter.WriteElementString( "EndDate", String.Empty );
      xmlWriter.WriteElementString( "DateLastAccessed", this.dateLastAccessed.ToString( culture ) );
    }
    #endregion AbstractConstraint implementation
    #region Properties
    /// <summary>
    /// Gets or Sets the start date/time for this constraint.
    /// </summary>
    /// <param>
    /// Sets the start date/time for this constraint.
    /// </param>
    /// <returns>
    ///	Gets the start date/time for this constraint.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or Sets the start date/time for this constraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public DateTime StartDate
    {
      get { return this.start; }
      set
      {
        if ( !this.StartDate.Equals( value ) )
        {
          this.start = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the end date/time for this constraint.
    /// </summary>
    /// <param>
    /// Sets the end date/time for this constraint.
    /// </param>
    /// <returns>
    ///	Gets the end date/time for this constraint.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or Sets the end date/time for this constraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public DateTime EndDate
    {
      get { return this.end; }
      set
      {
        if ( !this.EndDate.Equals( value ) )
        {
          this.end = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the duration in days this license should be valid from the first use.
    /// </summary>
    /// <param>
    /// Sets the duration in days this license should be valid from the first use.
    /// </param>
    /// <returns>
    ///	Gets the duration in days this license should be valid from the first use.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or Sets the duration in days this license should be valid from the first use." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int Duration
    {
      get { return this.maxDuration; }
      set
      {
        if ( !this.Duration.Equals( value ) )
        {
          this.maxDuration = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the URL, as a <c>String</c>, which points to where more information may be found.
    /// </summary>
    /// <param>
    /// Sets the URL, as a <c>String</c>, which points to where more information may be found.
    /// </param>
    /// <returns>
    ///	Gets the URL, as a <c>String</c>, which points to where more information may be found.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the URL, as a String, which points to where more information may be found." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string InfoURL
    {
      get { return this.infoUrl; }
      set
      {
        if ( !this.infoUrl.Equals( value ) )
        {
          this.infoUrl = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the URL, as a <c>String</c>, which points to where a license can be purchased from.
    /// </summary>
    /// <param>
    /// Sets the URL, as a <c>String</c>, which points to where a license can be purchased from.
    /// </param>
    /// <returns>
    ///	Gets the URL, as a <c>String</c>, which points to where a license can be purchased from.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the URL, as a String, which points to where a license can be purchased from." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string PurchaseURL
    {
      get { return this.purchaseUrl; }
      set
      {
        if ( !this.PurchaseURL.Equals( value ) )
        {
          this.purchaseUrl = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the Terms/Conditions this license is valid for.
    /// </summary>
    /// <param>
    /// Sets the Terms/Conditions this license is valid for.
    /// </param>
    /// <returns>
    ///	Gets the Terms/Conditions this license is valid for.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the Terms/Conditions this license is valid for." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Condition
    {
      get { return this.conditions; }
      set
      {
        if ( !this.Condition.Equals( value ) )
        {
          this.conditions = value;
          this.IsModified = true;
        }
      }
    }
    #endregion
  }
}
