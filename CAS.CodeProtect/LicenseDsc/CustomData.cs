using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
namespace UAOOI.CodeProtect.LicenseDsc
{
  /// <summary>
  /// The <c>CustomData</c> object inherits from the <see cref="AbstractLicenseData"/>.  The
  /// <c>CustomData</c> is used by developers to store unique custom data using a Key/Value
  /// pairing in a <c>StringDictionary</c>.
  /// </summary>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  /// <seealso cref="AbstractLicenseData">AbstractLicenseData</seealso>
  /// <seealso cref="System.Collections.Specialized.StringDictionary">StringDictionary</seealso>
  [Serializable]
  public class CustomData: AbstractLicenseData
  {
    #region private
    #region not used
    ///// <summary>
    ///// Adds the values of a <c>StringDictionary</c> to the <c>CustomData</c> object.
    ///// </summary>
    ///// <param>
    ///// A <c>StringDictionary</c> with the data to be added.
    ///// </param>
    //private void AddCustomData( StringDictionary data )
    //{
    //  if ( this.Items == null )
    //    this.data = data;
    //  else
    //  {
    //    foreach ( DictionaryEntry dictEntry in this.Items )
    //      this.Items.Add( (String)dictEntry.Key, (String)dictEntry.Value );
    //    isDirty = true;
    //  }
    //}
    #endregion not used
    private StringDictionary data = new StringDictionary();
    #endregion private
    #region creators
    /// <summary>
    /// This is a static method that creates an <c>CustomData</c> from the passed in XML
    /// <see cref="System.String">String</see>.
    /// </summary>
    /// <param>
    /// The <see cref="System.String">String</see> representing the XML data.
    /// </param>
    /// <returns>
    /// The <c>CustomData</c> created from parsing this <see cref="System.String">String</see>.
    /// </returns>
    private static CustomData FromXml( string xmlData )
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml( xmlData );
      return FromXml( xmlDoc.SelectSingleNode( "/CustomData" ) );
    }
    /// <summary>
    /// This is a static method that creates an <c>CustomData</c> from a <see cref="XmlNode">XmlNode</see>.
    /// </summary>
    /// <param>
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>CustomData</c>.
    /// </param>
    /// <returns>
    /// The <c>CustomData</c> created from this <see cref="XmlNode">XmlNode</see>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the license data is null.
    /// </exception>
    private static CustomData FromXml( XmlNode itemsNode )
    {
      if ( itemsNode == null )
        throw new ArgumentNullException( "The license data is null." );
      StringDictionary dictionary = new StringDictionary();
      XmlNodeList list = itemsNode.SelectNodes( "/CustomData" );
      for ( int i = 0; i < list.Count; i++ )
      {
        XmlNode key = ( (XmlNode)list[ i ] ).SelectSingleNode( "Key/text()" );
        XmlNode val = ( (XmlNode)list[ i ] ).SelectSingleNode( "Value/text()" );
        if ( key != null && val != null )
          dictionary.Add( key.Value, val.Value );
      }
      if ( dictionary.Count > 0 )
        return new CustomData( dictionary );
      else
        return new CustomData();
    }
    /// <summary>
    /// This initializes an empty <c>CustomData</c> object.
    /// </summary>
    public CustomData() : this( null ) { }
    /// <summary>
    /// This initializes a <c>CustomData</c> object with a given <c>StringCollection</c>.
    /// </summary>
    /// <param name="col">
    /// The string collection data to use.
    /// </param>
    private CustomData( StringDictionary col )
    {
      if ( col == null )
        data = new StringDictionary();
      else
        data = col;
    }
    #endregion
    #region internal
    /// <summary>
    /// Adds a key/value pair containing custom data.
    /// </summary>
    /// <param name="key">
    /// A <see cref="System.String">String</see> representing the key for this item.
    /// </param>
    /// <param name="val">
    /// A <see cref="System.String">String</see> representing the value for this item.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the input key is an empty <see cref="System.String">String</see>.
    /// </exception>
    internal void AddCustomData( string key, string val )
    {
      if ( this.data == null )
        this.data = new StringDictionary();
      if ( key == String.Empty )
        throw new ArgumentException( "Empty String is not a valid encYey value." );
      this.data.Add( key, val );
      this.IsModified = true;
    }
    #endregion
    #region AbstractLicenseData implementation
    /// <summary>
    /// This creates a <see cref="System.String">String</see> representing the
    /// XML form for this <c>CustomData</c> object.
    /// </summary>
    /// <returns>
    /// The <see cref="System.String">String</see> representing this <c>CustomData</c> object in it's XML form.
    /// </returns>
    public override string ToXmlString()
    {
      StringBuilder xmlString = new StringBuilder();
      XmlTextWriter xmlWriter = new XmlTextWriter( new StringWriter( xmlString ) );
      xmlWriter.Formatting = Formatting.Indented;
      xmlWriter.IndentChar = '\t';
      if ( data != null )
      {
        xmlWriter.WriteStartElement( "CustomData" );
        foreach ( DictionaryEntry dictEntry in data )
        {
          xmlWriter.WriteStartElement( "Item" );
          xmlWriter.WriteElementString( "Key", (String)dictEntry.Key );
          xmlWriter.WriteElementString( "Value", (String)dictEntry.Value );
          xmlWriter.WriteEndElement(); // Item
        }
        xmlWriter.WriteEndElement(); // CustomData
        xmlWriter.Flush();
      }
      xmlWriter.Close();
      return xmlString.ToString();
    }
    #endregion AbstractLicenseData implementation
    #region Properties
    /// <summary>
    /// Gets or Sets the <c>StringDictionary</c> of the <c>CustomData</c> object.
    /// </summary>
    /// <param>
    ///	Sets the <c>StringDictionary</c> of the <c>CustomData</c> object.
    /// </param>
    /// <returns>
    /// Gets the <c>StringDictionary</c> of the <c>CustomData</c> object.
    /// </returns>
    [
    Bindable( false ),
    Browsable( false ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets or Sets the StringDictionary of the CustomData object." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public StringDictionary Items
    {
      get
      {
        //Can't do adds
        //ListDictionary
        //NameObjectCollection
        //
        //Can't even open
        //StringDictionary
        //DictionaryEntry

        return this.data;
      }
      set
      {
        this.data = value;
        IsModified = true;
      }
    }
    #endregion
  }
}
