//<summary>
//  Title   : AbstractConstraint
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
//  mailto:techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

namespace CAS.Lib.CodeProtect.LicenseDsc
{
  /// <summary>
  /// 	<p><c>AbstractConstraint</c> is an abstract class which all licensing
  /// constraints are built from.  The <c>AbstractConstraint</c> defines
  /// the necessary items for a Constraint to be used by
  /// the <see cref="CodeProtectLP">CodeProtectLP</see>.  The
  /// provider then uses the constraints <c>Validate</c> function to
  /// enforce the Constraint.</p>
  /// </summary>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  /// <seealso cref="CodeProtectLP">CodeProtectLP</seealso>
  /// <seealso cref="System.String">String</seealso>
  /// <seealso cref="System.Xml.XmlDocument">XmlDocument</seealso>
  /// <seealso cref="System.Xml.XmlNode">XmlNode</seealso>
  public abstract class AbstractConstraint: AbstractLicenseData, IConstraint
  {
    #region private
    private LicenseFile license = null;
    private string name = String.Empty;
    private string description = String.Empty;
    private Icon icon = null;
    private int m_volumeConstrain = -1;
    private int m_runTimeConstrain = -1;
    private Guid m_TypeGuid = new Guid();
    /// <summary>
    /// Adds the failure reason text.
    /// </summary>
    /// <param name="details">The details about the failure if any.</param>
    protected internal virtual void AddFailureReasonText( string details )
    {
      if ( License != null )
        License.FailureReason = License.FailureReason + "\nThe constraint failed:\n " + details + "\n" + ToString();
    }
    /// <summary>
    /// Does the indent.
    /// </summary>
    /// <param name="cStr">The string builder class wher indent should be done</param>
    /// <param name="depth">The depth (Indentation  level).</param>
    protected internal void DoIndent( StringBuilder cStr, int depth )
    {
      if ( cStr == null )
        throw new ArgumentNullException( "cStr" );
      cStr.Append( new string( '\t', depth ) );
    }
    /// <summary>
    /// Prepares the text information about constraint.
    /// </summary>
    /// <param name="cStr">The stringBuilder class where information should be stored</param>
    /// <param name="additionalInformation">The additional information.</param>
    /// <param name="depth">The depth (Indentation  level).</param>
    protected internal virtual void PrepareTextInformationAboutConstraint( StringBuilder cStr, string additionalInformation,int depth )
    {
      if ( cStr == null )
        throw new ArgumentNullException( "cStr" );
      DoIndent( cStr, depth );
      cStr.AppendLine( "Constraint details:");
      DoIndent( cStr, depth );
      cStr.AppendLine( "Name:\t\t" + Name );
      DoIndent( cStr, depth );
      cStr.AppendLine( "Description:\t" + Description );
      DoIndent( cStr, depth );
      cStr.AppendLine( "Volume constrain:\t" + VolumeConstrain );
      DoIndent( cStr, depth );
      cStr.AppendLine( "Run time constrain:\t" + RunTimeConstrain );
      DoIndent( cStr, depth );
      cStr.AppendLine( "GUID:\t\t" + GUID );
      cStr.Append( additionalInformation );
    }
    #endregion private
    #region public abstract
    /// <summary>
    /// gets String from XML.
    /// </summary>
    /// <param name="xmlData">The XML data.</param>
    /// <param name="xPath">The xPath.</param>
    /// <param name="val">The value.</param>
    protected void StringFromXML( XmlNode xmlData, string xPath, ref string val )
    {
      XmlNode c_TextNode = xmlData.SelectSingleNode( xPath );
      if ( c_TextNode != null )
        val = c_TextNode.Value;
    }
    /// <summary>
    /// gets Int from XML.
    /// </summary>
    /// <param name="xmlData">The XML data.</param>
    /// <param name="xPath">The xPath.</param>
    /// <param name="val">The value.</param>
    protected void IntFromXML( XmlNode xmlData, string xPath, ref int val )
    {
      string c_val = string.Empty;
      StringFromXML( xmlData, xPath, ref c_val );
      if ( c_val != string.Empty )
        val = int.Parse( c_val );
    }
    /// <summary>
    /// Defines the validation method of this Constraint.  This
    /// is the method the Constraint uses to accept or reject a
    /// license request.
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>If <c>true</c> the constraint applies to the validated type.</returns>
    /// <remarks>
    /// If Guid is not set for the constraint it applies to all types, otherwise the
    /// constraint is validated only for the type having the equal Guid.  For block constrains
    /// like And and Or the composed constraint is validated if the block constraint Guid is not
    /// set or is equal to the validating type guid. If a constraint is not validating it
    /// is considered not valid.
    /// </remarks>
    public virtual bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !m_TypeGuid.Equals( new Guid() ) && !m_TypeGuid.Equals( typeToValidate.GUID ) )
        return false;
      vc = Math.Max( vc, m_volumeConstrain );
      rtc = Math.Max( rtc, m_runTimeConstrain );
      AddWorning();
      return true;
    }
    private void AddWorning()
    {
      StringBuilder wrnk = new StringBuilder( Warning );
      if ( wrnk.Length == 0 )
        return;
      foreach ( PropertyDescriptor item in TypeDescriptor.GetProperties( this ) )
      {
        object val = item.GetValue( this );
        string valText = val == null ? string.Empty : val.ToString();
        string token = string.Format( "<{0}>", item.Name );
        wrnk.Replace( token, valText );
      }
      License.AddWorning( wrnk.ToString() );
    }
    /// <summary>
    /// This is responsible for parsing a <see cref="System.String">String</see>
    /// to form the <c>BetaConstriant</c>.
    /// </summary>
    /// <param name="xmlData">
    /// The Xml data in the form of a <c>String</c>.
    /// </param>
    public void FromXml( string xmlData )
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml( xmlData );
      FromXml( xmlDoc.SelectSingleNode( "/Constraint" ) );
    }
    /// <summary>
    /// This loads the XML data for the Constraint from an
    /// <see cref="System.Xml.XmlNode">XmlNode</see>.  The <c>XmlNode</c>
    /// is the piece of the <see cref="System.Xml.XmlDocument">XmlDocument</see>
    /// that is contained within the constraint block of the
    /// <c>XmlDocument</c>.
    /// </summary>
    /// <param name="xmlData">
    /// A <c>XmlNode</c> representing the constraint
    /// of the <c>XmlDocument</c>.
    /// </param>
    public virtual void FromXml( XmlNode xmlData )
    {
      if ( xmlData == null )
        throw new ArgumentNullException( "The license data is null." );
      StringFromXML( xmlData, "Name/text()", ref name );
      StringFromXML( xmlData, "Description/text()", ref description );
      IntFromXML( xmlData, "VolumeConstrain/text()", ref m_volumeConstrain );
      IntFromXML( xmlData, "RunTimeConstrain/text()", ref m_runTimeConstrain );
      string xmlGUID = string.Empty;
      StringFromXML( xmlData, "GUID/text()", ref xmlGUID );
      if ( xmlGUID == string.Empty )
        m_TypeGuid = new Guid();
      else
        m_TypeGuid = new Guid( xmlGUID );
      string wr = string.Empty;
      StringFromXML( xmlData, "Warning/text()", ref wr );
      Warning = wr;
    }
    /// <summary>
    /// Destroys this instance of the Constraint.
    /// </summary>
    public void Dispose()
    {
    }
    #endregion public abstract
    #region AbstractLicenseData
    /// <summary>
    /// To the XML string.
    /// </summary>
    /// <param name="xmlWriter">The XML writer.</param>
    protected abstract void ToXmlString( XmlTextWriter xmlWriter );
    /// <summary>
    /// Converts this instance of License Data to a
    /// <see cref="System.String">String</see> representing the XML
    /// of the specific License Data object.
    /// </summary>
    /// <returns></returns>
    /// <return>
    /// The <c>String</c> representing this License Data.
    /// </return>
    public override string ToXmlString()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlTextWriter xmlWriter = new XmlTextWriter( new StringWriter( xmlString ) );
      xmlWriter.Formatting = Formatting.Indented;
      xmlWriter.WriteStartElement( "Constraint" );
      xmlWriter.WriteElementString( "Name", this.Name );
      xmlWriter.WriteElementString( "Type", this.GetType().ToString() );
      xmlWriter.WriteElementString( "Description", this.Description );
      xmlWriter.WriteElementString( "VolumeConstrain", m_volumeConstrain.ToString() );
      xmlWriter.WriteElementString( "RunTimeConstrain", m_runTimeConstrain.ToString() );
      xmlWriter.WriteElementString( "GUID", m_TypeGuid.ToString() );
      xmlWriter.WriteElementString( "Warning", Warning );
      ToXmlString( xmlWriter );
      xmlWriter.WriteEndElement(); // constraints
      xmlWriter.Close();
      return xmlString.ToString();
    }
    #endregion AbstractLicenseData
    #region Properties
    /// <summary>
    /// Gets or Sets <see cref="LicenseFile">LicenseFile</see> this constraint belongs to.
    /// </summary>
    /// <param>
    ///	Sets the <c>LicenseFile</c> this constraint belongs to.
    /// </param>
    /// <returns>
    ///	Gets the <c>LicenseFile</c> this constraint belongs to.
    /// </returns>
    [
    Bindable( false ),
    Browsable( false ),
    DefaultValue( 0 ),
    Description( "Gets or Sets LicenseFile this constraint belongs to." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public LicenseFile License
    {
      get
      {
        return this.license;
      }
      set
      {
        this.license = value;
      }
    }
    /// <summary>
    /// Gets or Sets the name of this constraint.
    /// </summary>
    /// <param>
    ///	Sets the name of this constraint.
    /// </param>
    /// <returns>
    ///	Gets the name of this constraint.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "About" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the name of this constraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value;
      }
    }
    /// <summary>
    /// Gets the name (based on type) of the constraint.
    /// </summary>
    /// <value>The name of the constraint.</value>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "About" ),
    DisplayName( "Constraint Type" ),
    DefaultValue( "" ),
    Description( "Provides information about the type of the constraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string ConstraintType
    {
      get { return this.GetType().Name; }
    }
    /// <summary>
    /// Gets or Sets the description of this constraint.
    /// </summary>
    /// <param>
    ///	Sets the description of this constraint.
    /// </param>
    /// <returns>
    ///	Gets the description of this constraint.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "About" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the description of this constraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( false )
    ]
    public string Description
    {
      get
      {
        return this.description;
      }
      set
      {
        this.description = value;
      }
    }
    /// <summary>
    /// Gets or Sets the icon for this constraint.
    /// </summary>
    /// <param>
    ///	Sets the icon for this constraint.
    /// </param>
    /// <returns>
    ///	Gets the icon for this constraint.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "About" ),
    DefaultValue( null ),
    Description( "Gets or Sets the icon for this constraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public System.Drawing.Icon Icon
    {
      get
      {
        return this.icon;
      }
      set
      {
        this.icon = value;
      }
    }
    /// <summary>
    /// Gets or Sets the number of items licensed program can instantiate, e.g. number of process variables, number of interfaces, etc.
    /// </summary>
    /// <param>
    ///	Sets the number of items.
    /// </param>
    /// <returns>
    ///	Number of items. -1 no constrain.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( "" ),
    Description( "Constrains number of items licensed program can instantiate, e.g. number of process variables, number of interfaces, etc." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( false )
    ]
    public int VolumeConstrain
    {
      get
      {
        return this.m_volumeConstrain;
      }
      set
      {
        this.m_volumeConstrain = value;
      }
    }
    /// <summary>
    /// Gets or Sets the runtime of the program in hours.
    /// </summary>
    /// <param>
    ///	Sets the the runtime.
    /// </param>
    /// <returns>
    ///	Runtime in hours. –1 means no limits.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( "" ),
    Description( "Limits the runtime of the program in hours." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( false )
    ]
    public int RunTimeConstrain
    {
      get
      {
        return this.m_runTimeConstrain;
      }
      set
      {
        this.m_runTimeConstrain = value;
      }
    }
    /// <summary>
    /// Gets or Sets the runtime of the program in hours.
    /// </summary>
    /// <param>
    ///	Sets the the runtime.
    /// </param>
    /// <returns>
    ///	Runtime in hours. –1 means no limits.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( "" ),
    Description(
      "If Guid is not set for the constraint it applies to all types, otherwise the constraint is " +
      "validated only for the type having the equal Guid.  For block constrains like And and Or the " +
      "composed constraints are validated if the block constraint Guid is not set or is equal to the " +
      "validating type guid. If a constraint is not validating it is considered not valid." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( false )
    ]
    public string GUID
    {
      get { return this.m_TypeGuid.ToString(); }
      set
      {
        string str = value.Trim();
        if ( str.Length > 0 )
          this.m_TypeGuid = new Guid( value );
        else
          this.m_TypeGuid = new Guid();
      }
    }
    /// <summary>
    /// Gets or sets the warning provided during evaluwation of relevant constrains.
    /// </summary>
    /// <value>The warning text.</value>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( "" ),
    Description(
      "If warning is set up (is not null or empty) it is added to the license warning collection every time the " +
      "constraint is evaluated valid. All tokens with syntax <PropertyName> are replaced with current value of " +
      "the appropriate property. " ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    ReadOnly( false )
    ]
    public string Warning { get; set; }
    #endregion //Properties
    #region override object
    /// <summary>
    /// Provides detailed information about the constrain as a text
    /// </summary>
    /// <returns>Detailed information about the constrain as a text</returns>
    public override string ToString()
    {
      StringBuilder cStr = new StringBuilder();
      PrepareTextInformationAboutConstraint( cStr, "", 0 );
      return cStr.ToString();
    }
    #endregion
  }
}
