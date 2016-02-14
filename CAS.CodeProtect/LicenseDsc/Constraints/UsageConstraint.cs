//<summary>
//  Title   : Usage Constraint
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
using System.Globalization;
using System.Text;
using System.Xml;
using CAS.Lib.CodeProtect.Properties;

namespace CAS.Lib.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>This <see cref='UsageConstraint'/> constrains the user
  /// to a usage limit.  It supports a Maximum Usage, Hit and Days count.  Once
  /// the maximum number is reached the license expires.</p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class UsageConstraint: AbstractConstraint
  {
    #region private
    private int maxUsageCount = -1;
    private int currentUsageCount = 0;
    private int maxHitCount = -1;
    private int currentHitCount = 0;
    private int maxDaysCount = -1;
    private int currentDaysCount = 1;
    private DateTime dateLastAccessed = DateTime.Now;
    #endregion private
    #region public creators
    /// <summary>
    /// This is the constructor for the <c>UsageConstraint</c>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public UsageConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>UsageConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public UsageConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Usage Constraint";
      base.Description = "This UsageConstraint constrains the user to a usage ";
      base.Description += "limit. It supports a Maximum Usage, Hit and Days ";
      base.Description += "count. Once the maximum number is reached the license ";
      base.Description += "expires.";
    }
    #endregion creators
    #region AbstractConstraint implementation
    /// <summary>
    /// Prepares the text information about constraint.
    /// </summary>
    /// <param name="cStr">The stringBuilder class where information should be stored</param>
    /// <param name="additionalInformation">The additional information.</param>
    /// <param name="depth">The depth (Indentation  level).</param>
    protected internal override void PrepareTextInformationAboutConstraint( StringBuilder cStr, string additionalInformation, int depth )
    {
      StringBuilder sb_addtionalInfo = new StringBuilder();
      if ( MaxDays > 0 )
      {
        DoIndent( sb_addtionalInfo, depth );
        sb_addtionalInfo.AppendLine( string.Format( Resources.UsageConstraint_Days, CurrentDays, MaxDays ) );
      }
      if ( MaxHitCount > 0 )
      {
        DoIndent( sb_addtionalInfo, depth );
        sb_addtionalInfo.AppendLine( string.Format( Resources.UsageConstraint_Hit, CurrentHitCount, MaxHitCount ) );
      }
      if ( MaxUsageCount > 0 )
      {
        DoIndent( sb_addtionalInfo, depth );
        sb_addtionalInfo.AppendLine( string.Format( Resources.UsageConstraint_Usage, CurrentUsageCount, MaxUsageCount ) );
      }
      base.PrepareTextInformationAboutConstraint( cStr, sb_addtionalInfo.ToString(), depth );
    }
    /// <summary>
    /// <p>This verifies the license meets its desired validation criteria. This includes
    /// validating that the license has not been used more times then the usage constraint
    /// allows. The usage constraints available are:
    /// <list>
    /// <item>Hit Count - The maximum number of hits for a web services.</item>
    /// <item>Usage Count - The maximum number of uses for an application.</item>
    /// <item>Duration - The maximum number of days this may be used.</item>
    /// </list>
    /// If any of these values are exceeded then the license validation will return false
    /// and the failure reason will be set.</p>
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria. Otherwise <c>False</c>.
    /// </returns>
    /// <remarks>
    /// When a failure occurs the FailureReason will be set to one of the following
    /// depending upon which one failed:
    /// <list my_Type="definition">
    ///  <listheader>
    ///   <term>Validation Type</term>
    ///   <description>Failure String Result</description>
    ///  </listheader>
    ///  <item>
    ///   <term>Hits</term>
    ///   <description>The maximum number of hits has been reached.</description>
    ///  </item>
    ///  <item>
    ///   <term>Usage</term>
    ///   <description>The maximum number of uses has been reached.</description>
    ///  </item>
    ///  <item>
    ///   <term>Duration</term>
    ///   <description>The maximum number of days has been reached.</description>
    ///  </item>
    /// </list>
    /// </remarks>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      this.IncrementDays();
      this.IncrementHits();
      this.IncrementUsage();
      if ( this.MaxUsageCount > 0 && this.CurrentUsageCount > this.MaxUsageCount )
      {
        AddFailureReasonText( "The maximum number of uses has been reached." );
        return false;
      }
      else if ( this.MaxHitCount > 0 && this.CurrentHitCount > this.MaxHitCount )
      {
        AddFailureReasonText( "The maximum number of hits has been reached." );
        return false;
      }
      else if ( this.MaxDays > 0 && this.CurrentDays > this.MaxDays )
      {
        AddFailureReasonText( "The maximum number of days has been reached." );
        return false;
      }
      return true;
    }
    /// <summary>
    /// This creates a <c>UsageConstriant</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>UsageConstriant</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode )
    {
      base.FromXml( itemsNode );
      XmlNode mUsageTextNode = itemsNode.SelectSingleNode( "MaxUsageCount/text()" );
      XmlNode cUsageTextNode = itemsNode.SelectSingleNode( "CurrentUsageCount/text()" );
      XmlNode mHitTextNode = itemsNode.SelectSingleNode( "MaxHitCount/text()" );
      XmlNode cHitTextNode = itemsNode.SelectSingleNode( "CurrentHitCount/text()" );
      XmlNode mDaysTextNode = itemsNode.SelectSingleNode( "MaxDaysCount/text()" );
      XmlNode cDaysTextNode = itemsNode.SelectSingleNode( "CurrentDaysCount/text()" );
      XmlNode dateAccessedTextNode = itemsNode.SelectSingleNode( "DateLastAccessed/text()" );

      if ( mUsageTextNode != null )
        this.maxUsageCount = Convert.ToInt32( mUsageTextNode.Value );
      if ( cUsageTextNode != null )
        this.currentUsageCount = Convert.ToInt32( cUsageTextNode.Value );
      if ( mHitTextNode != null )
        this.maxHitCount = Convert.ToInt32( mHitTextNode.Value );
      if ( cHitTextNode != null )
        this.currentHitCount = Convert.ToInt32( cHitTextNode.Value );
      if ( mDaysTextNode != null )
        this.maxDaysCount = Convert.ToInt32( mDaysTextNode.Value );
      if ( cDaysTextNode != null )
        this.currentDaysCount = Convert.ToInt32( cDaysTextNode.Value );
      if ( dateAccessedTextNode != null )
      {
        this.dateLastAccessed = DateTime.Parse( dateAccessedTextNode.Value, CultureInfo.InvariantCulture,
                              DateTimeStyles.NoCurrentDateDefault );
        //this.dateLastAccessed	= Convert.ToDateTime( dateAccessedTextNode.Value );
      }
    }
    /// <summary>
    /// Converts this <c>UsageConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the UsageConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter )
    {
      xmlWriter.WriteElementString( "MaxUsageCount", Convert.ToString( MaxUsageCount ) );
      xmlWriter.WriteElementString( "CurrentUsageCount", Convert.ToString( CurrentUsageCount ) );
      xmlWriter.WriteElementString( "MaxHitCount", Convert.ToString( MaxHitCount ) );
      xmlWriter.WriteElementString( "CurrentHitCount", Convert.ToString( CurrentHitCount ) );
      xmlWriter.WriteElementString( "MaxDaysCount", Convert.ToString( MaxDays ) );
      xmlWriter.WriteElementString( "CurrentDaysCount", Convert.ToString( CurrentDays ) );
      xmlWriter.WriteElementString( "DateLastAccessed", this.dateLastAccessed.ToString(CultureInfo.InvariantCulture) );
    }
    #endregion
    #region Properties
    /// <summary>
    /// Gets or Sets the maximum allowed uses for this <c>UsageConstraint</c>.
    /// </summary>
    /// <param>
    ///	Sets the maximum allowed uses for this <c>UsageConstraint</c>.
    /// </param>
    /// <returns>
    ///	Gets the maximum allowed uses for this <c>UsageConstraint</c>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( -1 ),
    Description( "Gets or Sets the maximum allowed uses for this UsageConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int MaxUsageCount
    {
      get
      {
        return this.maxUsageCount;
      }
      set
      {
        if ( !this.MaxUsageCount.Equals( value ) )
        {
          this.maxUsageCount = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets the current usage count for this <c>UsageConstraint</c>.
    /// </summary>
    /// <returns>
    ///	Gets the current usage count for this <c>UsageConstraint</c>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( false ),
    Category( "Data" ),
    DefaultValue( 0 ),
    Description( "Gets the current usage count for this UsageConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int CurrentUsageCount
    {
      get
      {
        return this.currentUsageCount;
      }
    }
    /// <summary>
    /// Increments the current usage counter by one.
    /// </summary>
    private void IncrementUsage()
    {
      this.currentUsageCount++;
    }
    /// <summary>
    /// Gets or Sets the maximum allowed hits for this <c>UsageConstraint</c>.
    /// </summary>
    /// <param>
    ///	Sets the maximum allowed hits for this <c>UsageConstraint</c>.
    /// </param>
    /// <returns>
    ///	Gets the maximum allowed hits for this <c>UsageConstraint</c>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( -1 ),
    Description( "Gets or Sets the maximum allowed hits for this UsageConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int MaxHitCount
    {
      get
      {
        return this.maxHitCount;
      }
      set
      {
        if ( !this.MaxHitCount.Equals( value ) )
        {
          this.maxHitCount = value;
          this.IsModified = true;
        }
      }
    }
    /// <summary>
    /// Gets the current hit count for this <c>UsageConstraint</c>.
    /// </summary>
    /// <returns>
    ///	Gets the current hit count for this <c>UsageConstraint</c>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( false ),
    Category( "Data" ),
    DefaultValue( 0 ),
    Description( "Gets the current hit count for this UsageConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int CurrentHitCount
    {
      get
      {
        return this.currentHitCount;
      }
    }
    /// <summary>
    /// Increments the current hit counter by one.
    /// </summary>
    private void IncrementHits()
    {
      this.currentHitCount++;
    }

    /// <summary>
    /// Gets or Sets the maximum number of days allowed for this <c>UsageConstraint</c>.
    /// </summary>
    /// <param>
    ///	Sets the maximum number of days allowed for this <c>UsageConstraint</c>.
    /// </param>
    /// <returns>
    ///	Gets the maximum number of days allowed for this <c>UsageConstraint</c>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( -1 ),
    Description( "Gets or Sets the maximum number of days allowed for this UsageConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int MaxDays
    {
      get
      {
        return this.maxDaysCount;
      }
      set
      {
        if ( !this.MaxDays.Equals( value ) )
        {
          this.maxDaysCount = value;
          this.IsModified = true;
        }
      }
    }

    /// <summary>
    /// Gets the current days count for this <c>UsageConstraint</c>.
    /// </summary>
    /// <returns>
    ///	Gets the current days count for this <c>UsageConstraint</c>.
    /// </returns>
    [
    Bindable( false ),
    Browsable( false ),
    Category( "Data" ),
    DefaultValue( 0 ),
    Description( "Gets the current days count for this UsageConstraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int CurrentDays
    {
      get
      {
        return this.currentDaysCount;
      }
    }

    /// <summary>
    /// Increments the days counter by one when a new day has occurred.
    /// </summary>
    private void IncrementDays()
    {
      if ( this.dateLastAccessed.Month != DateTime.Now.Month ||
          this.dateLastAccessed.Day != DateTime.Now.Day ||
          this.dateLastAccessed.Year != DateTime.Now.Year )
      {
        this.dateLastAccessed = DateTime.Now;
        this.currentDaysCount++;
        this.IsModified = true;
      }
    }
    #endregion
  }
}
