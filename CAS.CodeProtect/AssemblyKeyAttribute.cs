using System;
using System.ComponentModel;
using System.Reflection;
namespace CAS.Lib.CodeProtect
{
  /// <summary>
  /// This is an assembly attribute to be used to defined the encryption key that 
  /// should be used to decrypt the license file when it is encrypted.  If the value 
  /// is defined then the license file will be decrypted using the defined key.  If 
  /// the decrypt fails then an exception will be thrown. 
  /// </summary>
  /// <example>
  /// c#
  /// <code>
  /// &#91;assembly: CAS.Lib.CodeProtectAssemblyOpenLicenseKey("test")&#93;
  /// </code>
  /// vb#
  /// <code>
  /// &lt;assembly: CAS.Lib.CodeProtectAssemblyOpenLicenseKey("test")&gt;
  /// </code>
  /// </example>
  [
  AttributeUsage( AttributeTargets.Assembly )
  ]
  public class AssemblyKeyAttribute: System.Attribute
  {
    #region private
    private string encryptionKey = String.Empty;
    #endregion
    /// <summary>
    /// This is responsible for returning the encryption key defined in 
    /// the Assembly.  If not defined then an Empty string is returned.
    /// </summary>
    public static string GetEncryptionKeyAttribute( Type type )
    {
      string key = String.Empty;
      // Look for key defined in Assembly.
      AssemblyKeyAttribute attr =
        (AssemblyKeyAttribute)Attribute.GetCustomAttribute( Assembly.GetAssembly( type ), typeof( AssemblyKeyAttribute ) );
      if ( attr != null )
        return attr.EncryptionKey;
      else
        return String.Empty;
    }
    /// <summary>
    /// The constructor for an empty <c>AssemblyOpenLicenseKeyAttribute</c>.
    /// </summary>
    public AssemblyKeyAttribute() : this( "" ) { }
    /// <summary>
    /// The constructor for an <c>AssemblyOpenLicenseKeyAttribute</c> with a given encryption key.
    /// </summary>
    public AssemblyKeyAttribute( string key )
    {
      this.encryptionKey = key;
    }
    /// <summary>
    /// Gets the Encryption Key to be used for the license.
    /// </summary>
    /// <returns>
    /// Gets the Encryption Key to be used for the license.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Data" ),
    DefaultValue( "" ),
    Description( "Gets the Encryption Key to be used for the license." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public string EncryptionKey
    {
      get { return this.encryptionKey; }
    }
  }
}

