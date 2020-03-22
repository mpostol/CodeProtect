//<summary>
//  Title   : IConstraint
//  System  : Microsoft Visual C# .NET 2005
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//  History :
//    MPostol - 10-11-2006
//
//  Copyright (C)2006, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto:techsupp@UAOOI.eu
//  http://www.UAOOI.eu
//</summary>

using System.Xml;

namespace UAOOI.CodeProtect.LicenseDsc
{
  /// <summary>
  /// 	<p><c>IConstraint</c> is an interface class which all Constraints must inherit.  The
  /// <c>IConstraint</c> defines the necessary items for a Constraint to be used by the
  /// <see cref="CodeProtectLP">OpenLicenseProvider</see>.  The provider then uses
  /// the Constraint's Validate( ) function to enforce the Constraint.</p>
  /// </summary>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  /// <seealso cref="CodeProtectLP">OpenLicenseProvider</seealso>
  /// <seealso cref="System.String">String</seealso>
  /// <seealso cref="System.Xml.XmlDocument">XmlDocument</seealso>
  /// <seealso cref="System.Xml.XmlNode">XmlNode</seealso>
  public interface IConstraint
  {
    #region methods
    /// <summary>
    /// Defines the validation method of this Constraint.  This is the method the
    /// Constraint uses to accept or reject a license request.
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    bool Validate( System.Type typeToValidate, ref int vc, ref int rtc );
    /// <summary>
    /// This is used to create a Constraint from a <see cref="System.String">String</see>
    /// representing the Xml data of a Constraints node in the <see cref="LicenseFile"/>.
    /// </summary>
    /// <param name="xmlData">
    /// A <c>String</c> representing the XML data for this Constraint.
    /// </param>
    void FromXml(string xmlData);
    /// <summary>
    /// This loads the XML data for a Constraint from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// The <c>XmlNode</c> is the piece of the <see cref="System.Xml.XmlDocument">XmlDocument</see>
    /// that is contained within the Constraint block of the <c>XmlDocument</c>.
    /// </summary>
    /// <param name="xmlData">
    /// A <c>XmlNode</c> representing the Constraint of the <c>XmlDocument</c>.
    /// </param>
    void FromXml(XmlNode xmlData);
    /// <summary>
    /// Destroys this instance of the Constraint.
    /// </summary>
    void Dispose();
    /// <summary>
    /// Converts this instance of the Constraint to a <see cref="System.String">String</see>
    /// representing the Xml of the Constraint object.
    /// </summary>
    /// <return>
    /// The <c>String</c> representing this Constraint.
    /// </return>
    string ToXmlString();
    ///	<summary>
    /// Resets the is dirty flag.
    /// </summary>
    void Saved();
    #endregion
    #region Properties
    /// <summary>
    /// Gets or Sets the <see cref="LicenseFile">LicenseFile</see> this Constraint belongs to.
    /// </summary>
    /// <param>
    ///	Sets the <c>LicenseFile</c> this Constraint belongs to.
    /// </param>
    /// <returns>
    ///	Gets the <c>LicenseFile</c> this Constraint belongs to.
    /// </returns>
    LicenseFile License
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or Sets the name of this Constraint.
    /// </summary>
    /// <param>
    ///	Sets the name of this Constraint.
    /// </param>
    /// <returns>
    ///	Gets the name of this Constraint.
    /// </returns>
    string Name
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or Sets the description of this Constraint.
    /// </summary>
    /// <param>
    ///	Sets the description of this Constraint.
    /// </param>
    /// <returns>
    ///	Gets the description of this Constraint.
    /// </returns>
    string Description
    {
      get;
      set;
    }
    /// <summary>
    /// Gets or Sets the icon for this Constraint.
    /// </summary>
    /// <param>
    ///	Sets the icon for this Constraint.
    /// </param>
    /// <returns>
    ///	Gets the icon for this Constraint.
    /// </returns>
    System.Drawing.Icon Icon
    {
      get;
      set;
    }
    /// <summary>
    /// Gets if this Constraint has changed since the last save.
    /// </summary>
    /// <returns>
    /// Gets if this Constraint has changed since the last save.
    /// </returns>
    bool IsDirty
    {
      get;
    }
    #endregion
  }
}

