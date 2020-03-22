//<summary>
//  Title   : Version Constraint
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
using System.Text;
using System.Xml;

namespace UAOOI.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='VersionConstraint'/> constrains the user to only use the license
  /// with a given range of versions attached to an Assembly.  For example if an assembly
  /// version is 0.95.0.0 and the version range is 0.94.0.0 to 0.96.0.0 then the license
  /// will pass.  However if the assembly is then upgrade to version 1.0.0.0 then the license
  /// will expire.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class VersionConstraint: AbstractConstraint
  {
    #region private
    private Version minVersion = new Version();
    private Version maxVersion = new Version();
    #endregion private
    #region public creators
    /// <summary>
    /// This is the constructor for the <c>VersionConstraint</c>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public VersionConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>VersionConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public VersionConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Version Constraint";
      base.Description = "This VersionConstraint constrains the user to only use the license ";
      base.Description += "with a given range of versions attached to an Assembly.";
    }
    #endregion creators
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
      sb_addtionalInfo.AppendLine( "Minimum version:\t" + Minimum );
      DoIndent( sb_addtionalInfo, depth );
      sb_addtionalInfo.AppendLine( "Maximum version:\t" + Maximum );
      base.PrepareTextInformationAboutConstraint( cStr, sb_addtionalInfo.ToString(), depth );
    }
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
      if ( ( this.Minimum.Equals( new Version() ) && this.Maximum.Equals( new Version() ) ) )
        return true;
      if ( typeToValidate.Assembly.GetName().Version <= this.minVersion ||
          typeToValidate.Assembly.GetName().Version >= this.maxVersion )
      {
        AddFailureReasonText( "The current version is not within the constraints of this license." );
        return false;
      }
      return true;
    }
    /// <summary>
    /// This creates a <c>VersionConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>VersionConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode )
    {
      base.FromXml( itemsNode );
      XmlNode minVersionTextNode = itemsNode.SelectSingleNode( "MinimumVersion/text()" );
      XmlNode maxVersionTextNode = itemsNode.SelectSingleNode( "MaximumVersion/text()" );
      if ( minVersionTextNode != null )
        this.minVersion = new Version( minVersionTextNode.Value );
      if ( maxVersionTextNode != null )
        this.maxVersion = new Version( maxVersionTextNode.Value );
    }
    #endregion AbstractConstraint implementation
    #region AbstractLicenseData implementation
    /// <summary>
    /// Converts this <c>VersionConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the IConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter )
    {
      xmlWriter.WriteElementString( "MinimumVersion", this.Minimum.ToString() );
      xmlWriter.WriteElementString( "MaximumVersion", this.Maximum.ToString() );
    }
    #endregion  AbstractConstraint implementation
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
    public string Minimum
    {
      get { return this.minVersion.ToString(); }
      set
      {
        this.minVersion = new Version( value );
        this.IsModified = true;
      }
    }
    /// <summary>
    /// Gets or Sets the maximum version allowed for this license.
    /// </summary>
    /// <param>
    ///	Sets the maximum version allowed for this license.
    /// </param>
    /// <returns>
    ///	Gets the maximum version allowed for this license.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or sets the maximum version allowed for this license." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Maximum
    {
      get { return this.maxVersion.ToString(); }
      set
      {
        this.maxVersion = new Version( value );
        this.IsModified = true;
      }
    }
    #endregion
  }
}
