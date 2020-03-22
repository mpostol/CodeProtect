using System;
using System.ComponentModel;
using System.Xml;
namespace UAOOI.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='ProcessorConstraint'/> constrains the user to only use the license
  /// with a given OS or Processor</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class ProcessorConstraint: AbstractConstraint
  {
    #region public creators
    /// <summary>
    /// This is the constructor for the <c>ProcessorConstraint</c>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public ProcessorConstraint() : this( null ) { }

    /// <summary>
    /// This is the constructor for the <c>ProcessorConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public ProcessorConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Processor Constraint";
      base.Description = "This ProcessorConstraint constrains the user to only use the license ";
      base.Description += "with a given OS or Processor.";
    }
    #endregion public creators
    #region AbstractConstraint implementation
    /// <summary>
    /// <p>This verifies the license meets its desired validation criteria.  This includes
    /// validating that the assembly version is within the licenses defined range.</p>
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria.  Otherwise
    /// <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to: "The current version is not
    /// within the constraints of this license."
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      throw new ApplicationException( "Not implemented yet." );
    }
    /// <summary>
    /// This creates a <c>ProcessorConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>ProcessorConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode ) { base.FromXml( itemsNode ); }
    #endregion AbstractConstraint implementation
    #region AbstractLicenseData implementation
    /// <summary>
    /// Converts this <c>ProcessorConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the IConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter ) { }
    #endregion AbstractLicenseData implementation
    #region Properties
    /// <summary>
    /// Gets or Sets the minimum version allowed for this license.
    /// </summary>
    /// <param>
    ///	Sets the minimum version allowed for this license.
    /// </param>
    /// <returns>
    ///	Gets the minimum version allowed for this license.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( 0.0 ),
    Description( "Gets or sets the minimum version allowed for this license." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int OS
    {
      get { return -1; }
    }
    #endregion
  }
}

