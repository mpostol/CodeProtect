using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;
namespace UAOOI.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='BetaConstraint'/> constrains the user
  /// to a given time period.  It supports an end date that the license will expire.
  /// It also has the ability to show the user a link to download an update to the
  /// beta once it expires.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class BetaConstraint: AbstractConstraint
  {
    #region private
    private DateTime end = new DateTime();
    private String updateUrl = String.Empty;
    #endregion
    #region public constructors
    /// <summary>
    /// This is the constructor for the <c>BetaConstraint</c>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public BetaConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>BetaConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public BetaConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Beta Constraint";
      base.Description = "The BetaConstraint constrains the user to a given time ";
      base.Description += "period.  It supports an end date that the license will ";
      base.Description += "expire. It also has the ability to show the user a link ";
      base.Description += "to download an update to the beta once it expires.";
    }
    #endregion
    #region AbstractConstraint implementation
    /// <summary>
    /// <p>This verifies the license meets its desired validation criteria.  This includes
    /// validating that the license is before the defined end date.  If it is not then
    /// the license validation will return false and the failure reason will be set.</p>
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria.  Otherwise
    /// <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to: "The beta period has
    /// expired. You may get an update at: xxx"  The section "You may get an update
    /// at: xxx" will only be displayed if UpdateUrl has been assinged a value.
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      if ( EndDate.Ticks > 0 && DateTime.Now <= EndDate )
        return true;
      StringBuilder errStr = new StringBuilder();
      errStr.Append( "The beta period has expired." );
      if ( this.UpdateURL != String.Empty )
      {
        errStr.Append( "\n" );
        errStr.Append( "You may get an update at: " );
        errStr.Append( "\n" );
        errStr.Append( this.UpdateURL );
      }
      AddFailureReasonText( errStr.ToString() );
      return false;
    }
    /// <summary>
    /// This creates a <c>BetaConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>BetaConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode )
    {
      base.FromXml( itemsNode );
      XmlNode endDateTextNode = itemsNode.SelectSingleNode( "EndDate/text()" );
      XmlNode updateURLTextNode = itemsNode.SelectSingleNode( "UpdateUrl/text()" );
      if ( endDateTextNode != null )
        EndDate = DateTime.Parse( endDateTextNode.Value, culture, DateTimeStyles.NoCurrentDateDefault );
      if ( updateURLTextNode != null )
        UpdateURL = updateURLTextNode.Value;
    }
    #endregion AbstractConstraint implementation
    #region AbstractLicenseData implementation
    /// <summary>
    /// Converts this <c>BetaConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the BetaConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter )
    {
      xmlWriter.WriteElementString( "UpdateUrl", this.UpdateURL );
      if ( EndDate.Ticks != 0 )
        xmlWriter.WriteElementString( "EndDate", EndDate.ToString( culture ) );
      else
        xmlWriter.WriteElementString( "EndDate", String.Empty );
    }
    #endregion AbstractLicenseData implementation
    #region Properties
    /// <summary>
    /// Gets or Sets the end date/time for this <see cref="BetaConstraint">BetaConstraint</see>.
    /// </summary>
    /// <param>
    ///	Sets the end date/time for this <see cref="BetaConstraint">BetaConstraint</see>.
    /// </param>
    /// <returns>
    ///	Gets the end date/time for this <see cref="BetaConstraint">BetaConstraint</see>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or Sets the end date/time for this BetaConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public DateTime EndDate
    {
      get
      {
        return this.end;
      }
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
    /// Gets or Sets the URL, as a <see cref="System.String">String</see>, which
    /// points to where an update can be obtained.
    /// </summary>
    /// <param>
    ///	Sets the URL, as a <see cref="System.String">String</see>, which
    /// points to where an update can be obtained.
    /// </param>
    /// <returns>
    ///	Gets the URL, as a <see cref="System.String">String</see>, which
    /// points to where an update can be obtained.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the URL, as a String , which points to where an update can be obtained." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string UpdateURL
    {
      get
      {
        return this.updateUrl;
      }
      set
      {
        if ( !this.UpdateURL.Equals( value ) )
        {
          this.updateUrl = value;
          this.IsModified = true;
        }
      }
    }
    #endregion
  }
}
