using System;
namespace CAS.Lib.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='AndConstraint'/> contains a collection of constraints that 
  /// will be grouped together as a bitwise AND operation.  It is responsible for 
  /// validating the containing <c>IConstraints</c> and will be valid as long as 
  /// all the constraints contained are valid.  The purpose of this is to allow 
  /// a user to force multiple constraints to pass before allowing the license 
  /// to be valid.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class AndConstraint: AbstractContainerConstraint
  {
    #region creators
    /// <summary>
    /// This is the constructor for the <c>AndConstraint</c>.  The constructor
    /// is used to create the object without a valid license attached it to.
    /// </summary>
    public AndConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>AndConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public AndConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "And Constraint";
      base.Description = "This AndConstraint contains a collection of constraints that ";
      base.Description += "will be grouped together as a bitwise AND operation.  It is ";
      base.Description += "validating the containing IConstraints and will be valid ";
      base.Description += "all the constraints contained are valid.  The purpose of this ";
      base.Description += "is to allow a user to force multiple constraints to pass before ";
      base.Description += "allowing the license to be valid.";
    }
    #endregion creators
    #region AbstractConstraint implementation
    /// <summary>
    /// This verifies the license meets its desired validation criteria.  This test will
    /// pass providing all contained constraints are valid. If any one of them is invalid then
    /// the validate will fail and the license failure reason will be set to the first
    /// constraint which failed while being validated.
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// 	<c>True</c> if the license meets the validation criteria.  Otherwise <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to the details of the
    /// failure for the last constraint which failed.
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      bool res = true;
      for ( int i = 0; i < this.Items.Count; i++ )
        res = res && this.Items[ i ].Validate( typeToValidate, ref vc, ref rtc );
      if ( res )
        return true;
      AddFailureReasonText( "See previous description" );
      return false;
    }
    #endregion AbstractConstraint implementation
  }
}
