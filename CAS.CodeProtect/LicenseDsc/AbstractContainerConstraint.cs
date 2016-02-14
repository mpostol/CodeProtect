//<summary>
//  Title   : Abstract Container Constraint
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml;

namespace CAS.Lib.CodeProtect.LicenseDsc
{
  /// <summary>
  /// <p>This <see cref="AbstractContainerConstraint">AbstractContainerConstraint</see> 
  /// is used to define a container for other constraints.  This is used to provide
  /// a method to create grouping of constraints to provide bitwise operations.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  public abstract class AbstractContainerConstraint: AbstractConstraint, IConstraintItemProvider
  {
    #region private
    private List<IConstraint> constraints = new List<IConstraint>();
    #endregion
    #region protected
    /// <summary>
    /// Prepares the text information about constraint.
    /// </summary>
    /// <param name="cStr">The stringBuilder class where information should be stored</param>
    /// <param name="additionalInformation">The additional information.</param>
    /// <param name="depth">The depth (Indentation  level).</param>
    protected internal override void PrepareTextInformationAboutConstraint( StringBuilder cStr, string additionalInformation, int depth )
    {
      base.PrepareTextInformationAboutConstraint( cStr, "", depth );
      foreach ( IConstraint icn in constraints )
      {
        AbstractConstraint cn = icn as AbstractConstraint;
        if ( cn != null )
        {
          DoIndent( cStr, depth );
          cStr.AppendLine( "Member constraint:" );
          cn.PrepareTextInformationAboutConstraint( cStr, "", depth + 1 );
          cStr.AppendLine();
        }
      }
    }
    /// <summary>
    /// Parse the XML content of the constraints group/fields section of the license.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="System.Xml.XmlNode">XmlNode</see> representing the
    /// Constraints List (System.Collections.Generic.List) section of the
    /// license.
    /// </param>
    protected void parseConstraintsFields( XmlNode itemsNode )
    {
      // Check if custom fields are defined
      if ( itemsNode == null )
        return;
      // If they are then process all of them
      XmlNodeList constraints = itemsNode.ChildNodes;
      for ( int i = 0; i < constraints.Count; i++ )
      {
        XmlNode constraint = constraints[ i ];
        XmlNode typeTextNode = constraint.SelectSingleNode( "Type/text()" );
        if ( typeTextNode != null )
        {
          Type constraintType = Type.GetType( typeTextNode.Value, false, true );
          ConstructorInfo cInfo = constraintType.GetConstructor( new Type[] { typeof( LicenseFile ) } );
          AbstractConstraint c = (AbstractConstraint)cInfo.Invoke( new Object[] { this.License } );
          c.FromXml( constraint );
          this.Items.Add( c );
        }
      }
    }
    #endregion
    #region AbstractConstraint implementation
    /// <summary>
    /// This creates an <c>AndConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>AndConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode )
    {
      if ( itemsNode == null )
        throw new ArgumentNullException( "The license data is null." );
      base.FromXml( itemsNode );
      parseConstraintsFields( itemsNode );
    }
    #endregion AbstractConstraint implementation
    #region AbstractLicenseData implementation
    /// <summary>
    /// Converts this <c>AndConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the IConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter )
    {
      for ( int i = 0; i < this.Items.Count; i++ )
        xmlWriter.WriteRaw( Items[ i ].ToXmlString() );
    }
    /// <summary>
    /// Toes the XML string.
    /// </summary>
    /// <returns></returns>
    public override string ToXmlString()
    {
      if ( this.Items == null || this.Items.Count == 0 )
        return string.Empty;
      return base.ToXmlString();
    }
    #endregion AbstractLicenseData implementation
    #region Properties
    /// <summary>
    /// Gets or Sets the <c>ConstraintCollection</c> for this ContainerConstraint.
    /// </summary>
    /// <param>
    /// Sets the <c>ConstraintCollection</c> for this ContainerConstraint.
    /// </param>
    /// <returns>
    ///	Gets the <c>ConstraintCollection</c> for this ContainerConstraint.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or Sets the ConstraintCollection for this ContainerConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ReadOnly( true )
    ]
    public List<IConstraint> Items
    {
      get { return this.constraints; }
      set { this.constraints = value; }
    }
    #endregion
  }
}
