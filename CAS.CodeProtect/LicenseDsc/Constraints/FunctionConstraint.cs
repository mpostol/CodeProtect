//___________________________________________________________________________________
//
//  Copyright (C) 2020, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using CAS.Lib.CodeProtect.Properties;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace CAS.Lib.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='FunctionConstraint'/> constrains use of selected function.
  /// It has the ability to show the user a link to download an update to the
  /// beta once it expires.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class FunctionConstraint : AbstractConstraint
  {
    #region private
    private bool m_Allows = true;
    private string updateUrl = string.Empty;
    #endregion

    #region public constructors
    /// <summary>
    /// This is the constructor for the <see cref="FunctionConstraint"/>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public FunctionConstraint() : this(null) { }
    /// <summary>
    /// This is the constructor for the <see cref="FunctionConstraint"/>. The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public FunctionConstraint(LicenseFile license)
    {
      base.License = license;
      base.Name = Resources.FunctionConstraintName;
      base.Description = Resources.FunctionConstraintDescriptor;
    }
    #endregion
    #region AbstractConstraint implementation
    /// <summary>
    /// Prepares the text information about constraint.
    /// </summary>
    /// <param name="cStr">The stringBuilder class where information should be stored</param>
    /// <param name="additionalInformation">The additional information.</param>
    /// <param name="depth">The depth (Indentation  level).</param>
    protected internal override void PrepareTextInformationAboutConstraint(StringBuilder cStr, string additionalInformation, int depth)
    {
      StringBuilder sb_addtionalInfo = new StringBuilder();
      DoIndent(sb_addtionalInfo, depth);
      if (Allow)
        sb_addtionalInfo.AppendLine(Resources.FunctionConstraint_Allowed);
      else
        sb_addtionalInfo.AppendLine(Resources.FunctionConstraint_NOTAllowed);
      base.PrepareTextInformationAboutConstraint(cStr, sb_addtionalInfo.ToString(), depth);
    }
    /// <summary>
    /// <p>This verifies the license meets its desired validation criteria. This includes
    /// validating that Allow flag is set to true. If it is not set the license validation will return false 
    /// and the failure reason will be set.</p>
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria.  Otherwise
    /// <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to: 
    /// <code>
    /// This license does not allow you to execute the functionality {Name}.
    /// You may get an update at:
    /// \t{UpdateURL}
    /// </code>
    /// </remarks>
    public override bool Validate(Type typeToValidate, ref int vc, ref int rtc)
    {
      if (!base.Validate(typeToValidate, ref vc, ref rtc))
        return false;
      if (m_Allows)
        return true;
      StringBuilder errStr = new StringBuilder();
      errStr.Append($"{Resources.LicMessageFunctionNotAllowed} {this.Name}; {Resources.GetUpgrade} {this.UpdateURL}");
      errStr.AppendLine();
      AddFailureReasonText(errStr.ToString());
      return false;
    }
    /// <summary>
    /// This creates a <see cref="FunctionConstraint"/> from an <see cref="System.Xml.XmlNode"/>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode"></see>/see> representing the <see cref="FunctionConstraint"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml(XmlNode itemsNode)
    {
      base.FromXml(itemsNode);
      XmlNode updateURLTextNode = itemsNode.SelectSingleNode("UpdateUrl/text()");
      if (updateURLTextNode != null)
        UpdateURL = updateURLTextNode.Value;
      XmlNode allowed = itemsNode.SelectSingleNode("Allow/text()");
      if (allowed != null)
        Allow = Convert.ToBoolean(allowed.Value);
    }
    #endregion AbstractConstraint implementation
    #region AbstractLicenseData implementation
    /// <summary>
    /// Converts this <see cref="FunctionConstraint"/> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the FunctionConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString(XmlTextWriter xmlWriter)
    {
      xmlWriter.WriteElementString("UpdateUrl", this.UpdateURL);
      xmlWriter.WriteElementString("Allow", Convert.ToString(Allow));
    }
    #endregion AbstractLicenseData implementation
    #region Properties
    /// <summary>
    /// Gets or Sets the end date/time for this <see cref="FunctionConstraint">FunctionConstraint</see>.
    /// </summary>
    /// <param>
    ///	Sets the end date/time for this <see cref="FunctionConstraint">FunctionConstraint</see>.
    /// </param>
    /// <returns>
    ///	Gets the end date/time for this <see cref="FunctionConstraint">FunctionConstraint</see>.
    /// </returns>
    [
    Bindable(false),
    Browsable(true),
    Category("Constraints"),
    DefaultValue(null),
    Description("Gets or sets the allow property. If the value is false the constrain is not valid"),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
    ]
    public bool Allow
    {
      get => this.m_Allows;
      set
      {
        if (!this.m_Allows.Equals(value))
        {
          this.m_Allows = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the URL, as a <see cref="System.String"/>, which
    /// points to where an update can be obtained.
    /// </summary>
    /// <value>The new URL.</value>
    /// <returns>
    /// Returns the URL, as a <see cref="string"/>, which points to where an update can be obtained.
    /// </returns>
    [
    Bindable(false),
    Browsable(true),
    Category("Data"),
    DefaultValue(""),
    Description("Gets or Sets the URL, as a String, which points to where an update can be obtained."),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
    ]
    public string UpdateURL
    {
      get => this.updateUrl;
      set
      {
        if (!this.UpdateURL.Equals(value))
        {
          this.updateUrl = value;
          this.IsModified = true;
        }
      }
    }
    #endregion
  }
}
