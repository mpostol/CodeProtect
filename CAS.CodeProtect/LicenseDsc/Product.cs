//<summary>
//  Title   : The <c>Product</c> is the information about the assembly this license is used for. 
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//  Copyright (C)2009, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@UAOOI.eu
//  http://www.UAOOI.eu
//</summary>

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using UAOOI.CodeProtect.Properties;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace UAOOI.CodeProtect.LicenseDsc
{
  /// <summary>
  ///  The <c>Product</c> is the information about the assembly this license is used for.  It
  /// contains values for the Assembly, name, version, etc... The <c>Product</c> object inherits 
  /// from the <see cref="AbstractLicenseData"/>.
  /// </summary>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  /// <seealso cref="AbstractLicenseData">AbstractLicenseData</seealso>
  [Serializable]
  public class Product: AbstractUpgradeableData<Product>
  {
    #region private
    // Product information
    private bool isInGac = false;
    private string filePath = String.Empty;
    private string shortName = String.Empty;
    private string fullName = String.Empty;
    private string version = String.Empty;
    private string developer = String.Empty;
    private string description = String.Empty;
    private bool isLicensed = false;
    /// <summary>
    /// This initializes a <c>Product</c> with the passed in values.
    /// </summary>
    /// <param name="gac">
    /// True if the assembly is found in the GAC.  Otherwise false
    /// </param>
    /// <param name="path">
    /// The path to the location where the Assembly was found.
    /// </param>
    /// <param name="sname">
    /// The short name of this <c>Assembly</c>.
    /// </param>
    /// <param name="fname">
    /// The full name of this <c>Assembly</c>.
    /// </param>
    /// <param name="ver">
    /// The version of this <c>Assembly</c>.
    /// </param>
    /// <param name="dev">
    /// The developer of this <c>Assembly</c>
    /// </param>
    /// <param name="desc">
    /// The description of the <c>Product</c>.
    /// </param>
    /// <param name="isLicensed">
    /// True if this <c>Product</c> should be completely licensed.  Otherwise false.
    /// </param>
    private Product( bool gac, string path, string sname, string fname, string ver, string dev, string desc, bool isLicensed )
    {
      this.IsInGAC = gac;
      this.FilePath = path;
      this.ShortName = sname;
      this.FullName = fname;
      this.Version = ver;
      this.Developer = dev;
      this.Description = desc;
      this.isLicensed = isLicensed;
      this.IsModified = false;
    }
    ///// <summary>
    ///// Dodaje do pliku licencji rozszerzenie .lic. Mozna podmienic rozszerzenie zmieniajac my_licExtension.
    ///// </summary>
    ///// <param name="myAss">
    ///// 
    ///// </param>
    ///// <returns></returns>
    //private string LicFileNameOf( Assembly myAss ) { return CreateAsseblyShortName( myAss ) + my_licExtension; }
    #endregion
    #region public
    /// <summary>
    /// This is a static method that creates an <c>Product</c> from the passed in XML
    /// <see cref="System.String">String</see>.
    /// </summary>
    /// <param>
    /// The <see cref="System.String">String</see> representing the Xml data.
    /// </param>
    /// <returns>
    /// The <c>Product</c> created from parsing this <see cref="System.String">String</see>.
    /// </returns>
    internal static Product FromXml( String xmlData )
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml( xmlData );
      return FromXml( xmlDoc.SelectSingleNode( "/Product" ) );
    }
    /// <summary>
    /// This is a static method that creates an <c>Product</c> from a <see cref="XmlNode">XmlNode</see>.
    /// </summary>
    /// <param>
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>Product</c>.
    /// </param>
    /// <returns>
    /// The <c>Product</c> created from this <see cref="XmlNode">XmlNode</see>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the license data is null.
    /// </exception>
    internal static Product FromXml( XmlNode itemsNode )
    {
      if ( itemsNode == null )
        throw new ArgumentNullException( "The license data is null." );
      bool isInGac = false;
      string filePath = String.Empty;
      string shortName = String.Empty;
      string fullName = String.Empty;
      string version = String.Empty;
      string developer = String.Empty;
      string description = String.Empty;
      bool isLicensed = false;
      XmlNode assemblyTextNode = itemsNode.SelectSingleNode( "Assembly/text()" );
      XmlNode gacTextNode = itemsNode.SelectSingleNode( "IsInGac/text()" );
      XmlNode filePathTextNode = itemsNode.SelectSingleNode( "FilePath/text()" );
      XmlNode snameTextNode = itemsNode.SelectSingleNode( "ShortName/text()" );
      XmlNode fnameTextNode = itemsNode.SelectSingleNode( "FullName/text()" );
      XmlNode versionTextNode = itemsNode.SelectSingleNode( "Version/text()" );
      XmlNode devevloperTextNode = itemsNode.SelectSingleNode( "Developer/text()" );
      XmlNode descriptionTextNode = itemsNode.SelectSingleNode( "Description/text()" );
      XmlNode isLicensedTextNode = itemsNode.SelectSingleNode( "IsLicensed/text()" );
      if ( filePathTextNode != null )
        filePath = filePathTextNode.Value;
      if ( gacTextNode != null )
        isInGac = Convert.ToBoolean( (string)gacTextNode.Value );
      if ( snameTextNode != null )
        shortName = snameTextNode.Value;
      if ( fnameTextNode != null )
        fullName = fnameTextNode.Value;
      if ( versionTextNode != null )
        version = versionTextNode.Value;
      if ( devevloperTextNode != null )
        developer = devevloperTextNode.Value;
      if ( descriptionTextNode != null )
        description = descriptionTextNode.Value;
      if ( isLicensedTextNode != null )
        isLicensed = Convert.ToBoolean( (string)isLicensedTextNode.Value );
      return new Product( isInGac, filePath, shortName, fullName, version, developer, description, isLicensed );
    }
    internal void SetProduct( DeployManifest product )
    {
      this.FullName = product.AssemblyIdentity.Name;
      this.ShortName = product.Product;
      this.Version = product.AssemblyIdentity.Version;
      this.IsModified = true;
    }
    #endregion
    #region creators
    /// <summary>
    /// This initializes an empty <c>Product</c>.
    /// </summary>
    internal Product() : this( false, "", "", "", "", "", "", false ) { }
    /// <summary>
    /// This initializes a <c>Product</c> with the passed in value.
    /// </summary>
    public Product( Assembly assembly )
    {
      AssemblyName name = assembly.GetName();
      this.IsInGAC = assembly.GlobalAssemblyCache;
      this.FilePath = string.Empty;
      this.ShortName = name.Name;
      this.FullName = name.FullName;
      this.Version = name.Version.ToString();
      this.Developer = string.Empty;
      this.Description = string.Empty;
      this.isLicensed = false;
      this.IsModified = false;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Product"/> class.
    /// </summary>
    /// <param name="Manifest">The manifest.</param>
    public Product( DeployManifest Manifest )
    {
      SetProduct( Manifest );
      this.IsInGAC = false;
      this.Developer = string.Empty;
      this.Description = string.Empty;
      this.isLicensed = false;
      this.IsModified = false;
    }

    #endregion
    #region Abstract class implementatiom
    /// <summary>
    /// This creates a <see cref="System.String">String</see> representing the
    /// XML form for this <c>Product</c>.
    /// </summary>
    /// <returns>
    /// The <see cref="System.String">String</see> representing this <c>Product</c> in it's XML form.
    /// </returns>
    public override string ToXmlString()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlTextWriter xmlWriter = new XmlTextWriter( new StringWriter( xmlString ) );
      XmlDocument tempDoc = new XmlDocument();
      xmlWriter.Formatting = Formatting.Indented;
      xmlWriter.IndentChar = '\t';
      //create nodes
      XmlNode productNode = tempDoc.CreateElement( "", "Product", "" );
      XmlNode assemblyNode = tempDoc.CreateElement( "", "Assembly", "" );
      XmlNode gacNode = tempDoc.CreateElement( "", "IsInGac", "" );
      XmlNode pathNode = tempDoc.CreateElement( "", "FilePath", "" );
      XmlNode snameNode = tempDoc.CreateElement( "", "ShortName", "" );
      XmlNode fnameNode = tempDoc.CreateElement( "", "FullName", "" );
      XmlNode versionNode = tempDoc.CreateElement( "", "Version", "" );
      XmlNode developerNode = tempDoc.CreateElement( "", "Developer", "" );
      XmlNode descNode = tempDoc.CreateElement( "", "Description", "" );
      XmlNode licensedNode = tempDoc.CreateElement( "", "IsLicensed", "" );
      //assign values
      assemblyNode.InnerText = this.shortName;
      gacNode.InnerText = this.IsInGAC.ToString();
      pathNode.InnerText = this.FilePath;
      snameNode.InnerText = this.ShortName;
      fnameNode.InnerText = this.FullName;
      versionNode.InnerText = this.Version;
      developerNode.InnerText = this.Developer;
      descNode.InnerText = this.Description;
      licensedNode.InnerText = this.IsLicensed.ToString();
      //append nodes
      productNode.AppendChild( assemblyNode );
      productNode.AppendChild( gacNode );
      productNode.AppendChild( pathNode );
      productNode.AppendChild( snameNode );
      productNode.AppendChild( fnameNode );
      productNode.AppendChild( versionNode );
      productNode.AppendChild( developerNode );
      productNode.AppendChild( descNode );
      productNode.AppendChild( licensedNode );
      tempDoc.AppendChild( productNode );
      //write out the result
      tempDoc.WriteTo( xmlWriter );
      xmlWriter.Flush();
      xmlWriter.Close();
      return xmlString.ToString();
    }
    /// <summary>
    /// Upgrade the product description if posible. Description, Developer and IsLicensed is upgraded 
    /// if provided by the new l;icense.
    /// </summary>
    /// <param name="upgrade">Instance of <see cref="Product"/> to be used to upgrade this product description.</param>
    /// <exception cref="LicenseFileException"> If name or version do not much.</exception>
    public override bool UpgardeData( Product upgrade )
    {
      if ( upgrade.ShortName.CompareTo( this.ShortName ) != 0 )
        throw new LicenseFileException( Resources.ErrStr_UpgardeData_Product_Name );
      if ( new Version( upgrade.Version ) < new Version( this.Version ) )
        throw new LicenseFileException( Resources.ErrStr_UpgardeData_Product_Version );
      UpgradeField( ref this.description, upgrade.Description );
      UpgradeField( ref this.developer, upgrade.Developer );
      isLicensed = this.isLicensed || upgrade.IsLicensed;
      return true;
    }
    #endregion AbstractULicenseData implementation
    #region Properties
    /// <summary>
    /// Generates license file name as [assembly name].lic
    /// </summary>
    /// <returns>file name</returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( null ),
    Description( "Gets or Sets the Assembly for this Product." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( true )
    ]
    public string LicFileName
    {
      get { return ShortName + ".lic"; }
    }
    /// <summary>
    /// Gets or Sets if this Assembly can be found in the Global Assembly Cache (GAC).
    /// </summary>
    /// <param>
    /// Sets if this Assembly can be found in the Global Assembly Cache (GAC).
    /// </param>
    /// <returns>
    /// Gets if this Assembly can be found in the Global Assembly Cache (GAC).
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( false ),
    Description( "Gets or Sets if this Assembly can be found in the Global Assembly Cache (GAC)." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public bool IsInGAC
    {
      get
      {
        return this.isInGac;
      }
      set
      {
        if ( this.isInGac != value )
        {
          this.isInGac = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the file path for this Assembly.
    /// </summary>
    /// <param>
    /// Sets the file path for this Assembly.
    /// </param>
    /// <returns>
    /// Gets the file path for this Assembly.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the file path for this Assembly." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( false )
    ]
    public string FilePath
    {
      get
      {
        return this.filePath;
      }
      set
      {
        if ( value != this.filePath )
        {
          this.filePath = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the short name of this Assembly.
    /// </summary>
    /// <param>
    /// Sets the short name of this Assembly.
    /// </param>
    /// <returns>
    /// Gets the short name of this Assembly.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the short name of this Assembly." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ]
    public string ShortName
    {
      get
      {
        return this.shortName;
      }
      set
      {
        if ( value != this.shortName )
        {
          this.shortName = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets or Sets the full name of this Assembly.
    /// </summary>
    /// <param>
    /// Sets the full name of this Assembly.
    /// </param>
    /// <returns>
    /// Gets the full name of this Assembly.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the full name of this Assembly." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( true )
    ]
    public string FullName
    {
      get
      {
        return this.fullName;
      }
      set
      {
        if ( value != this.fullName )
        {
          this.fullName = value;
          this.IsModified = true;
        }
      }
    }

    /// <summary>
    /// Gets or Sets the version of this Assembly.
    /// </summary>
    /// <param>
    /// Sets the version of this Assembly.
    /// </param>
    /// <returns>
    /// Gets the version of this Assembly.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the version of this Assembly." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( false )
    ]
    public string Version
    {
      get
      {
        return this.version;
      }
      set
      {
        if ( value == this.version )
          return;
        this.version = new Version( value ).ToString();
        this.IsModified = true;
      }
    }

    /// <summary>
    /// Gets or Sets the developer of this Assembly.
    /// </summary>
    /// <param>
    /// Sets the developer of this Assembly.
    /// </param>
    /// <returns>
    /// Gets the developer of this Assembly.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the developer of this Assembly." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Developer
    {
      get
      {
        return this.developer;
      }
      set
      {
        if ( value != this.developer )
        {
          this.developer = value;
          this.IsModified = true;
        }
      }
    }

    /// <summary>
    /// Gets or Sets a description for this <c>Product</c>.
    /// </summary>
    /// <param>
    /// Sets a description for this <c>Product</c>.
    /// </param>
    /// <returns>
    /// Gets a description for this <c>Product</c>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets a description for this Product." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Description
    {
      get
      {
        return this.description;
      }
      set
      {
        if ( value != this.description )
        {
          this.description = value;
          this.IsModified = true;
        }
      }
    }

    /// <summary>
    /// Gets or Sets if this <c>Product</c> has been fully licensed with no restrictions.
    /// </summary>
    /// <param>
    /// Sets if this <c>Product</c> has been fully licensed with no restrictions.
    /// </param>
    /// <returns>
    /// Gets if this <c>Product</c> has been fully licensed with no restrictions.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( false ),
    Description( "Gets or Sets if this <c>Product</c> has been fully licensed with no restrictions." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public bool IsLicensed
    {
      get
      {
        return this.isLicensed;
      }
      set
      {
        if ( value != this.isLicensed )
        {
          this.isLicensed = value;
          this.IsModified = true;
        }
      }
    }
    #endregion
  }
}
