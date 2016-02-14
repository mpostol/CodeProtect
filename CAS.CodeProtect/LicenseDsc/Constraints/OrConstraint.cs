using System;
namespace CAS.Lib.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='OrConstraint'/> contains a collection of constraints that 
  /// will be grouped together as a bitwise OR operation.  It is responsible for 
  /// validating the containing <c>IConstraints</c> and will be valid as long as 
  /// one of the constraints contained is valid.  The purpose of this is to allow 
  /// multiple constraints to be added and create a run as long as one is valid 
  /// scheme.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class OrConstraint: AbstractContainerConstraint
  {
    #region creators
    /// <summary>
    /// This is the constructor for the <c>OrConstraint</c>.  The constructor
    /// is used to create the object without a valid license attached it to.
    /// </summary>
    public OrConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>OrConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public OrConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Or Constraint";
      base.Description = "This OrConstraint contains a collection of constraints that will ";
      base.Description += "be grouped together as a bitwise OR operation.  It is responsible ";
      base.Description += "for validating the containing IConstraints and will be valid as ";
      base.Description += "long as one of the constraints contained is valid.  The purpose of ";
      base.Description += "this is to allow multiple constraints to be added and create a run ";
      base.Description += "as long as one is valid scheme.";
    }
    #endregion creators
    #region AbstractConstraint implementation
    /// <summary>
    /// This verifies the license meets its desired validation criteria.  This test will 
    /// pass as long as one of the constraints contained within it's collection is valid.
    /// If all of them are invalid, then the validate will fail and the license failure 
    /// reason will be set to the last constraint which failed while being validated.
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
    /// failure for the last constraint which failed.
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      bool val = false;
      for ( int i = 0; i < this.Items.Count; i++ )
        val = val || this.Items[ i ].Validate( typeToValidate, ref vc, ref rtc );
      if ( val )
      {
        if ( base.License != null )
          base.License.FailureReason = String.Empty;
        return true;
      }
      AddFailureReasonText("See previous descriptions");
      return false;
    }
    #endregion AbstractConstraint implementation
  }
}
