using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Xml;
namespace CAS.Lib.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='DomainConstraint'/> constrains the user to running within a
  /// given set of Domains.  This is primarily used for web based licensing to make
  /// sure that the license is only used for specific domains.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class DomainConstraint: AbstractConstraint
  {
    #region private
    private StringCollection domainCollection = new StringCollection();
    private string currentDomain = String.Empty;
    private Uri currentDomainUri = null;
    private HttpContext context = null;
    #endregion
    #region public creators
    /// <summary>
    /// This is the constructor for the <c>DomainConstraint</c>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public DomainConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>DomainConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    /// <remarks>
    /// This constructor initializes the current domain.  When initializing this
    /// an exception can be thrown.  The constructor will catch the exception
    /// and set the current domain to an empty string.  The license can handle this
    /// by setting one of the domains to an empty string which should allow the
    /// validation to pass.  This call will throw an exception only when used in
    /// a non web environment.
    /// </remarks>
    public DomainConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Domain Constraint";
      base.Description = "This DomainConstraint constrains the user to running within a given ";
      base.Description += "set of Domains.  This is primarily used for web based licensing to make ";
      base.Description += "sure that the license is only used for specific domains.";
      if ( HttpContext.Current == null )
        this.CurrentDomain = String.Empty;
      else
        try
        {
          this.context = HttpContext.Current;
          this.currentDomainUri = this.context.Request.Url;
          this.CurrentDomain = this.currentDomainUri.Host;
        }
        catch { this.CurrentDomain = String.Empty; }
    }
    #endregion
    #region AbstractConstraint implementation
    /// <summary>
    /// <p>This verifies the license meets its desired validation criteria.  This includes
    /// validating that the license is run within the set of given domains.  If the license
    /// is able to match a domain with the current domain then it will be successful (true).
    /// Otherwise it will return false and set the failure reason to the reason for the
    /// failure.</p>
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria.  Otherwise
    /// <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to: "The current domain is not
    /// supported by this license."
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      foreach ( String s in this.domainCollection )
      {
        if ( ( s.ToLower() ).Equals( this.CurrentDomain ) )
          return true;
      }
      AddFailureReasonText( "The current domain is not supported by this license." );
      return false;
    }
    /// <summary>
    /// This creates a <c>DomainConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>DomainConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode )
    {
      base.FromXml( itemsNode );
      XmlNodeList domainListNode = itemsNode.SelectNodes( "Domain" );
      if ( this.domainCollection == null && domainListNode.Count > 0 )
        this.domainCollection = new StringCollection();
      for ( int i = 0; i < domainListNode.Count; i++ )
        this.domainCollection.Add( ( (XmlNode)domainListNode[ i ] ).InnerText );
    }
    #endregion
    #region AbstractLicenseData implementation
    /// <summary>
    /// Converts this <c>DomainConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the DomainConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter )
    {
      for ( int i = 0; i < this.domainCollection.Count; i++ )
        xmlWriter.WriteElementString( "Domain", this.domainCollection[ i ] );
    }
    #endregion AbstractLicenseData implementation
    #region Properties
    /// <summary>
    /// Gets or Sets the an array of strings which represent the domain names allowed to use this license.
    /// </summary>
    /// <param>
    /// Sets the an array of strings which represent the domain names allowed to use this license.
    /// </param>
    /// <returns>
    ///	Gets the an array of strings which represent the domain names allowed to use this license.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or Sets the an array of strings which represent the domain names allowed to use this license." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string[] Domains
    {
      get
      {
        string[] tempArray = new string[ this.domainCollection.Count ];
        for ( int i = 0; i < this.domainCollection.Count; i++ )
        {
          tempArray[ i ] = this.domainCollection[ i ];
        }
        return tempArray;
      }
      set
      {
        this.domainCollection.AddRange( value );
        this.IsModified = true;
      }
    }
    /// <summary>
    /// Gets or Sets the current domain this license is being executed on.
    /// </summary>
    /// <param>
    /// Sets the current domain this license is being executed on.
    /// </param>
    /// <returns>
    ///	Gets the current domain this license is being executed on.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the current domain this license is being executed on." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string CurrentDomain
    {
      get
      {
        return this.currentDomain;
      }
      set
      {
        if ( ( (String)value ).StartsWith( "http" ) )
        {
          value = ( (String)value ).TrimEnd( "/".ToCharArray() );
          this.currentDomain = ( ( (String)value ).TrimStart( "http://".ToCharArray() ) ).ToLower();
        }
        else
        {
          this.currentDomain = value.ToLower();
        }
      }
    }
    #endregion
  }
}