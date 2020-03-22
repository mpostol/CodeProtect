using System;
using System.ComponentModel;
using System.Globalization;
namespace UAOOI.CodeProtect.LicenseDsc
{
	/// <summary>
	/// <p><c>AbstractLicenseData</c> is an abstract class which all licensing
	/// data, other then Constraints) must inherit.  The <c>AbstractLicenseData</c>
	/// defines the necessary items for any license data to be used by
	/// the <see cref="LicenseFile">LicenseFile</see>.</p>
	/// </summary>
	/// <seealso cref="ILicenseData">ILicenseData</seealso>
	/// <seealso cref="LicenseFile">LicenseFile</seealso>
	/// <seealso cref="System.String">String</seealso>
	/// <seealso cref="System.Xml.XmlDocument">XmlDocument</seealso>
	/// <seealso cref="System.Xml.XmlNode">XmlNode</seealso>
	public abstract class AbstractLicenseData : ILicenseData
	{
    /// <summary>
    /// Culture used to covert date and time
    /// </summary>
    protected readonly IFormatProvider culture = DateTimeFormatInfo.InvariantInfo;
    /// <summary>
		/// Used to denote if this object has changed and hasn't yet been saved.
		/// </summary>
		internal protected bool	IsModified = false;
		/// <summary>
		/// Converts this instance of License Data to a
		/// <see cref="System.String">String</see> representing the XML
		/// of the specific License Data object.
		/// </summary>
		/// <return>
		/// The <c>String</c> representing this License Data.
		/// </return>
		public abstract string ToXmlString( );
		///	<summary>
		/// Resets the is dirty flag to know this item has been saved.
		/// </summary>
		public void Saved( )
		{
			this.IsModified = false;
		}
		/// <summary>
		/// Gets if this object has changed since the last save.
		/// </summary>
		/// <returns>
		/// Gets if this object has changed since the last save.
		/// </returns>
		[
		Bindable(false),
		Browsable(false),
		Category("Data"),
		DefaultValue(false),
		Description( "Gets if this object has changed since the last save." ),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public bool IsDirty
		{
			get
			{
				return this.IsModified;
			}
		}
	}
}

