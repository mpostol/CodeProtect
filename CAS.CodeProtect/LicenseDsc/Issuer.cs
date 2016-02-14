using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
namespace CAS.Lib.CodeProtect.LicenseDsc
{
  /// <summary>
  /// The <c>Issuer</c> object inherits from the <see cref="AbstractLicenseData"/>.  The
  /// <c>Issuer</c> is used as the person who released the license.  The issue contains
  /// the person's name, email address and URL.
  /// </summary>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  /// <seealso cref="AbstractLicenseData">AbstractLicenseData</seealso>
  [Serializable]
  public class Issuer: AbstractUpgradeableData<Issuer>
  {
    #region private
    private string fullName = "cas.eu";
    private string email = "techsupp.cas.eu";
    private string url = "www.cas.eu";
    #endregion private
    #region internal
    /// <summary>
    /// This is a static method that creates an <c>Issuer</c> from a <see cref="XmlNode">XmlNode</see>.
    /// </summary>
    /// <param>
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>Issuer</c>.
    /// </param>
    /// <returns>
    /// The <c>Issuer</c> created from this <see cref="XmlNode">XmlNode</see>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the license data is null.
    /// </exception>
    internal static Issuer FromXml( XmlNode itemsNode )
    {
      if ( itemsNode == null )
        throw new ArgumentNullException( "The license data is null." );
      string fullName = String.Empty;
      string url = String.Empty;
      string email = String.Empty;
      XmlNode nameTextNode = itemsNode.SelectSingleNode( "FullName/text()" );
      XmlNode emailTextNode = itemsNode.SelectSingleNode( "Email/text()" );
      XmlNode urlTextNode = itemsNode.SelectSingleNode( "Url/text()" );
      if ( nameTextNode != null )
        fullName = nameTextNode.Value;
      if ( emailTextNode != null )
        email = emailTextNode.Value;
      if ( urlTextNode != null )
        url = urlTextNode.Value;
      return new Issuer( fullName, email, url );
    }
    #endregion internal
    #region creators
    /// <summary>
    /// This initializes an empty <c>Issuer</c>.
    /// </summary>
    public Issuer(){ }
    /// <summary>
    /// This initializes an <c>Issuer</c> with a given name, email and URL.
    /// </summary>
    /// <param name="name">
    /// The name of the person or company issuing the license.
    /// </param>
    /// <param name="email">
    /// The email of the person or company issuing the license. Generally this is a support email address.
    /// </param>
    /// <param name="url">
    /// The url of the person or company issuing the license. Generally this is the company's web address.
    /// </param>
    public Issuer( string name, string email, string url )
    {
      this.fullName = name;
      this.email = email;
      this.url = url;
    }
    #endregion creators
    #region AbstractLicenseData implementation
    /// <summary>
    /// This creates a <see cref="System.String">String</see> representing the
    /// XML form for this <c>Issuer</c>.
    /// </summary>
    /// <returns>
    /// The <see cref="System.String">String</see> representing this <c>Issuer</c> in it's XML form.
    /// </returns>
    public override string ToXmlString()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlTextWriter xmlWriter = new XmlTextWriter( new StringWriter( xmlString ) );
      xmlWriter.Formatting = Formatting.Indented;
      xmlWriter.IndentChar = '\t';
      xmlWriter.WriteStartElement( "Issuer" );
      xmlWriter.WriteElementString( "FullName", this.FullName );
      xmlWriter.WriteElementString( "Email", this.Email );
      xmlWriter.WriteElementString( "Url", this.Url );
      xmlWriter.WriteEndElement(); // Issuer
      xmlWriter.Close();
      return xmlString.ToString();
    }
    #endregion AbstractLicenseData implementation
    #region AbstractULicenseData implementation
    /// <summary>
    /// Upgrade this object
    /// </summary>
    /// <param name="upgrade">Object to be used to upgrade this object</param>
    /// <returns>false if upgrade is impossible due to inconsistency</returns>
    public override bool UpgardeData( Issuer upgrade )
    {
      UpgradeField( ref this.email, upgrade.Email );
      UpgradeField( ref this.fullName, upgrade.FullName );
      UpgradeField( ref this.url, upgrade.Url );
      return true;
    }
    #endregion AbstractULicenseData implementation
    #region properties
    /// <summary>
    /// Gets or Sets the name of the <c>Issuer</c> of this license.
    /// </summary>
    /// <param>
    ///	Gets the name of the <c>Issuer</c> of this license.
    /// </param>
    /// <returns>
    /// Sets the name of the <c>Issuer</c> of this license.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the name of the Issuer of this license." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string FullName
    {
      get { return this.fullName; }
      set
      {
        if ( this.fullName != value )
        {
          this.fullName = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the email address of the <c>Issuer</c> for this license.
    /// </summary>
    /// <param>
    ///	Gets the email address of the <c>Issuer</c> for this license.
    /// </param>
    /// <returns>
    /// Sets the email address of the <c>Issuer</c> for this license.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the email address of the Issuer for this license." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Email
    {
      get
      {
        return this.email;
      }
      set
      {
        if ( this.email != value )
        {
          this.email = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the URL of the <c>Issuer</c> for this license.
    /// </summary>
    /// <param>
    ///	Sets the URL of the <c>Issuer</c> for this license.
    /// </param>
    /// <returns>
    /// Gets the URL of the <c>Issuer</c> for this license.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the URL of the Issuer for this license." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Url
    {
      get { return this.url; }
      set
      {
        if ( this.url != value )
        {
          this.url = value;
          this.IsModified = true;
        }
      }
    }
    #endregion
  }
}
