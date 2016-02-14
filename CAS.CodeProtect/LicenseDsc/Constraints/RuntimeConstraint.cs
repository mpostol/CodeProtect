using System;
using System.ComponentModel;
using System.Xml;
namespace CAS.Lib.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='RuntimeConstraint'/> constrains the user to only running
  /// the license in a runtime environment.  If it is not in a Runtime
  /// environment then this constraint will fail.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class RuntimeConstraint: AbstractConstraint
  {
    #region public creators
    /// <summary>
    /// This is the constructor for the <c>RuntimeConstraint</c>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public RuntimeConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>RuntimeConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public RuntimeConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Runtime Constraint";
      base.Description = "This RuntimeConstraint constrains the user to only running ";
      base.Description += "the license in a runtime environment.  If it is not in a ";
      base.Description += "Runtime environment then this constraint will fail.";
    }
    #endregion public creators
    #region AbstractConstraint implementation
    /// <summary>
    /// <p>This verifies the license meets its desired validation criteria.  This includes
    /// validating that the license is being used in the context of Runtime. If it is
    /// not then the license validation will return false and the failure reason will be
    /// set.</p>
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria.  Otherwise <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to: "The license may only be used
    /// in a Runtime environment. Design Time licensing is not supported."
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      if ( LicenseManager.CurrentContext.UsageMode != LicenseUsageMode.Runtime )
      {
        AddFailureReasonText
          ( "The license may only be used in a Runtime environment. Design Time licensing is not supported." );
        return false;
      }
      return true;
    }
    /// <summary>
    /// This creates a <c>RuntimeConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>RuntimeConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode ) { base.FromXml( itemsNode ); }
    #endregion AbstractConstraint implementation
    #region AbstractLicenseData implementation
    /// <summary>
    /// Converts this <c>RuntimeConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the RuntimeConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter ) { }
    #endregion AbstractLicenseData implementation
  }
}
