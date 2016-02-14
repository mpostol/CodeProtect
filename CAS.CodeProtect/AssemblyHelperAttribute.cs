//<summary>
//  Title   :CAS.Lib.CodeProtect.AssemblyHelperAttribute: to be used to defined the product.
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2009, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;
using System.ComponentModel;
using CAS.Lib.CodeProtect.Properties;

namespace CAS.Lib.CodeProtect
{
  /// <summary>
  /// This is an assembly attribute to be used to defined the product and 
  /// company information.  This information will be used when an exception
  /// is thrown because a valid license couldn't be found.  It may also be 
  /// used by a developer to access their information to display in a 
  /// product's about page. 
  /// </summary>
  /// <example>
  /// c#
  /// <code>
  /// &#91;assembly: CAS.Lib.CodeProtectAssemblyOpenLicenseHelper("Open License", "SP extreme")&#93;
  /// </code>
  /// vb#
  /// <code>
  /// &lt;assembly: CAS.Lib.CodeProtectAssemblyOpenLicenseHelper("Open License", "SP extreme")&gt;
  /// </code>
  /// </example>
  [
  AttributeUsage( AttributeTargets.Assembly )
  ]
  public class AssemblyHelperAttribute: System.Attribute
  {
    #region private
    private string product = String.Empty;
    private string company = String.Empty;
    private string url = String.Empty;
    private string email = String.Empty;
    private string phone = String.Empty; 
    #endregion

    #region public
    /// <summary>
    /// The constructor for an empty <c>AssemblyOpenLicenseHelperAttribute</c>.
    /// </summary>
    public AssemblyHelperAttribute() : this( "", String.Empty, String.Empty, String.Empty, String.Empty ) { }
    /// <summary>
    /// The constructor for an <c>AssemblyOpenLicenseHelperAttribute</c>.
    /// </summary>
    /// <param name="product">
    /// The name of the product this licensing scheme is for.
    /// </param>
    public AssemblyHelperAttribute( string product ) : this( product, String.Empty, String.Empty, String.Empty, String.Empty ) { }
    /// <summary>
    /// The constructor for an <c>AssemblyOpenLicenseHelperAttribute</c>.
    /// </summary>
    /// <param name="product">
    /// The name of the product this licensing scheme is for.
    /// </param>
    /// <param name="company">
    /// The name of the company who developed this product.
    /// </param>
    public AssemblyHelperAttribute( string product, string company )
      : this( product, company, String.Empty, String.Empty, String.Empty ) { }
    /// <summary>
    /// The constructor for an <c>AssemblyOpenLicenseHelperAttribute</c>.
    /// </summary>
    /// <param name="product">
    /// The name of the product this licensing scheme is for.
    /// </param>
    /// <param name="company">
    /// The name of the company who developed this product.
    /// </param>
    /// <param name="url">
    /// The URL of the company.
    /// </param>
    public AssemblyHelperAttribute( string product, string company, string url )
      : this( product, company, url, String.Empty, String.Empty ) { }
    /// <summary>
    /// The constructor for an <c>AssemblyOpenLicenseHelperAttribute</c>.
    /// </summary>
    /// <param name="product">
    /// The name of the product this licensing scheme is for.
    /// </param>
    /// <param name="company">
    /// The name of the company who developed this product.
    /// </param>
    /// <param name="url">
    /// The URL of the company.
    /// </param>
    /// <param name="email">
    /// An email address to contact the company. Generally this is a support email address.
    /// </param>
    public AssemblyHelperAttribute( string product, string company, string url, string email )
      : this( product, company, url, email, String.Empty ) { }
    /// <summary>
    /// The constructor for an <c>AssemblyOpenLicenseHelperAttribute</c>.
    /// </summary>
    /// <param name="product">
    /// The name of the product this licensing scheme is for.
    /// </param>
    /// <param name="company">
    /// The name of the company who developed this product.
    /// </param>
    /// <param name="url">
    /// The URL of the company.
    /// </param>
    /// <param name="email">
    /// An email address to contact the company. Generally this is a support email address.
    /// </param>
    /// <param name="phone">
    /// A phone number to contact the company. Generally this is a support phone number.
    /// </param>
    public AssemblyHelperAttribute( string product, string company, string url, string email, string phone )
    {
      this.Product = product;
      this.Company = company;
      this.Url = url;
      this.Email = email;
      this.Phone = phone;
    }
    #region Properties
    /// <summary>
    /// Gets or Sets the name of the product this licensing scheme is for.
    /// </summary>
    /// <param>
    /// Sets the name of the product this licensing scheme is for.
    /// </param>
    /// <returns>
    /// Gets the name of the product this licensing scheme is for.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the name of the product this licensing scheme is for." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Product
    {
      get
      {
        return this.product;
      }
      set
      {
        this.product = value;
      }
    }

    /// <summary>
    /// Gets or Sets the name of the company who developed this product.
    /// </summary>
    /// <param>
    /// Sets the name of the company who developed this product.
    /// </param>
    /// <returns>
    /// Gets the name of the company who developed this product.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the name of the company who developed this product." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Company
    {
      get
      {
        return this.company;
      }
      set
      {
        this.company = value;
      }
    }

    /// <summary>
    /// Gets or Sets the URL of the company's web address who developed this product.
    /// </summary>
    /// <param>
    /// Sets the URL of the company's web address who developed this product.
    /// </param>
    /// <returns>
    /// Gets the URL of the company's web address who developed this product.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the URL of the company's web address who developed this product." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Url
    {
      get
      {
        return this.url;
      }
      set
      {
        this.url = value;
      }
    }

    /// <summary>
    /// Gets or Sets an email address to contact the company. Generally this is a support email address.
    /// </summary>
    /// <param>
    /// Gets an email address to contact the company.
    /// </param>
    /// <returns>
    /// Gets an email address to contact the company.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets an email address to contact the company." ),
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
        this.email = value;
      }
    }

    /// <summary>
    /// Gets or Sets phone number to contact the company. Generally this is a support phone number.
    /// </summary>
    /// <param>
    /// Sets phone number to contact the company. 
    /// </param>
    /// <returns>
    /// Gets phone number to contact the company.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets phone number to contact the company." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Phone
    {
      get
      {
        return this.phone;
      }
      set
      {
        this.phone = value;
      }
    }
    #endregion
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      string cmpnMsg = string.IsNullOrEmpty( Company ) ? "the manufacturer " : this.Company;
      string urlMsg = String.Empty;
      if ( !String.IsNullOrEmpty( Url ) )
        urlMsg = string.Format( "( {0} )", this.Url );
      string emlMsg = String.Empty;
      if ( !String.IsNullOrEmpty( Email ) )
        emlMsg = string.Format( "at {0} ", this.Email );
      string phnMsg = string.Empty;
      if ( !String.IsNullOrEmpty( Phone ) )
        phnMsg = string.Format( "or by phone at {0} ", this.Phone );
      return string.Format( Resources.AssemblyHelperToString, cmpnMsg, urlMsg, emlMsg, phnMsg );
    } 
    #endregion
  }
}

