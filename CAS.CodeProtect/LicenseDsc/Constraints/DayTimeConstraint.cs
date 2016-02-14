using System;
using System.Collections;
using System.ComponentModel;
using System.Xml;
namespace CAS.Lib.CodeProtect.LicenseDsc.Constraints
{
  /// <summary>
  /// <p>The <see cref="DayTimeConstraint">DayTimeConstraint</see> constrains the user
  /// to only using this license during a period of time within a day.  I.E. A user may
  /// use the license between midnight and 7am and 5pm and midnight - Non business hours.
  /// </p>
  /// </summary>
  /// <seealso cref="AbstractConstraint">AbstractConstraint</seealso>
  /// <seealso cref="IConstraint">IConstraint</seealso>
  /// <seealso cref="LicenseFile">LicenseFile</seealso>
  public class DayTimeConstraint: AbstractConstraint
  {
    /// <summary>
    /// The time range values collection.
    /// </summary>
    private TimeRangeCollection timeRange = new TimeRangeCollection();
    /// <summary>
    /// This is the constructor for the <c>DayTimeConstraint</c>.  The constructor
    /// is used to create the object with a valid license to attach it to.
    /// </summary>
    public DayTimeConstraint() : this( null ) { }
    /// <summary>
    /// This is the constructor for the <c>DayTimeConstraint</c>.  The constructor
    /// is used to create the object and assign it to the proper license.
    /// </summary>
    /// <param name="license">
    /// The <see cref="LicenseFile">LicenseFile</see> this constraint
    /// belongs to.
    /// </param>
    public DayTimeConstraint( LicenseFile license )
    {
      base.License = license;
      base.Name = "Day Time Constraint";
      base.Description = "The DayTimeConstraint constrains the user ";
      base.Description += "to only using this license during a period of time within a day.  I.E. A user may ";
      base.Description += "use the license between midnight and 7am and 5pm and midnight - Non business hours.";
    }
    /// <summary>
    /// This verifies the license is allowed to be used during the current time range.
    /// </summary>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="vc">Volume constraint - max number of items. -1 no constrain.</param>
    /// <param name="rtc">Runtime in hours. –1 means no limits.</param>
    /// <returns>
    /// <c>True</c> if the license meets the validation criteria.  Otherwise
    /// <c>False</c>.
    /// </returns>
    public override bool Validate( Type typeToValidate, ref int vc, ref int rtc )
    {
      if ( !base.Validate( typeToValidate, ref vc, ref rtc ) )
        return false;
      Time currentMoment = Time.Now();
      //Loop through each entry
      for ( int i = 0; i < this.timeRange.Count; i++ )
        if ( ( (TimeRange)this.timeRange[ i ] ).Start.MilitaryTime <= currentMoment.MilitaryTime &&
            ( (TimeRange)this.timeRange[ i ] ).End.MilitaryTime >= currentMoment.MilitaryTime )
          return true;
      AddFailureReasonText( "" );
      return false;
    }
    /// <summary>
    /// This creates a <c>DayTimeConstraint</c> from an <see cref="System.Xml.XmlNode">XmlNode</see>.
    /// </summary>
    /// <param name="itemsNode">
    /// A <see cref="XmlNode">XmlNode</see> representing the <c>DayTimeConstraint</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <see cref="XmlNode">XmlNode</see> is null.
    /// </exception>
    public override void FromXml( XmlNode itemsNode )
    {
      base.FromXml( itemsNode );
      XmlNodeList rangeListNode = itemsNode.SelectNodes( "Range" );
      if ( this.timeRange == null && rangeListNode.Count > 0 )
        this.timeRange = new TimeRangeCollection();
      for ( int i = 0; i < rangeListNode.Count; i++ )
      {
        XmlAttributeCollection attrColection = ( (XmlNode)rangeListNode[ i ] ).Attributes;
        XmlAttribute startTimeNode = (XmlAttribute)attrColection.GetNamedItem( "StartTime" );
        XmlAttribute endTimeNode = (XmlAttribute)attrColection.GetNamedItem( "EndTime" );
        Time start = null;
        Time end = null;
        if ( startTimeNode != null )
          start = new Time( Convert.ToInt32( startTimeNode.Value ) );
        if ( endTimeNode != null )
          end = new Time( Convert.ToInt32( endTimeNode.Value ) );
        this.timeRange.Add( new TimeRange( start, end ) );
      }
    }
    /// <summary>
    /// Converts this <c>DayTimeConstraint</c> to an Xml <c>String</c>.
    /// </summary>
    /// <returns>
    /// A <c>String</c> representing the DayTimeConstraint as Xml data.
    /// </returns>
    protected override void ToXmlString( XmlTextWriter xmlWriter )
    {
      for ( int i = 0; i < this.timeRange.Count; i++ )
      {
        TimeRange r = this.timeRange[ i ];
        xmlWriter.WriteStartElement( "Range", "" );
        xmlWriter.WriteAttributeString( "StartTime", "", r.Start.MilitaryTime.ToString() );
        xmlWriter.WriteAttributeString( "EndTime", "", r.End.MilitaryTime.ToString() );
        xmlWriter.WriteEndElement();
      }
    }
    #region Properties
    /// <summary>
    /// Gets or Sets the time range to be used for this constraint.
    /// </summary>
    /// <param>
    ///	Sets the time range to be used for this constraint.
    /// </param>
    /// <returns>
    /// Gets the time range to be used for this constraint.
    /// </returns>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Constraints" ),
    DefaultValue( null ),
    Description( "Gets or Sets the time range to be used for this constraint." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public TimeRangeCollection Items
    {
      get
      {
        return this.timeRange;
      }
      set
      {
        this.timeRange = value;
        this.IsModified = true;
      }
    }
    #endregion
  }
  /// <summary>
  /// <p>The Time object is a rough implementation of military time to be used as the
  /// time range.  This will eventually change but for now it serves the purpose of
  /// defining a time range using 0 to 2400.  This class also has the ability to convert
  /// the current DateTime.Now to a Time object value.</p>
  /// </summary>
  public class Time
  {
    private int timeValue = -1; //Military time value 0000 - 2400
    /// <summary>
    /// A static method to return the current Time.  This uses the DateTime.Now call
    /// and then converts it to a Time object.
    /// </summary>
    public static Time Now()
    {
      DateTime d = DateTime.Now;

      int timeValue = d.Hour * 100;
      timeValue = timeValue + d.Minute;

      return new Time( timeValue );
    }
    /// <summary>
    /// This will create a Time object with no time value being set
    /// </summary>
    public Time() : this( -1 ) { }
    /// <summary>
    /// This will create a time value with the passed in time value.
    /// </summary>
    public Time( int timeValue )
    {
      this.timeValue = timeValue;
    }
    /// <summary>
    /// Gets or Sets the current Military time value.  Military time is in the range of 0 to 2400.
    /// </summary>
    /// <param>
    ///	Sets the current Military time value.
    /// </param>
    /// <returns>
    ///	Gets the current Military time value.
    /// </returns>
    /// <exception cref="System.ApplicationException">
    /// This will throw an exception if the time value is out of range (0 to 2400).
    /// </exception>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Time" ),
    DefaultValue( 0 ),
    Description( "Gets or Sets the current Military time value.  Military time is in the range of 0 to 2400." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public int MilitaryTime
    {
      get
      {
        return this.timeValue;
      }
      set
      {
        if ( value < 0 || value > 2400 )
        {
          throw new System.ApplicationException( "The time must be in valid military time standard ( 0 to 2400)." );
        }
        this.timeValue = value;
      }
    }
  }
  /// <summary>
  /// This is a range of Time values to provide a start/end time.
  /// </summary>
  public class TimeRange
  {
    private Time start = null;
    private Time end = null;
    /// <summary>
    /// Initialized a Time Range with a start time of 0 and an end time of 2400.
    /// </summary>
    public TimeRange() : this( new Time( 0 ), new Time( 2400 ) ) { }
    /// <summary>
    /// Initialized a Time Range with a given start time and an end time of 2400.
    /// </summary>
    public TimeRange( Time start ) : this( start, new Time( 2400 ) ) { }
    /// <summary>
    /// Initialized a Time Range with a the given start and stop time.
    /// </summary>
    public TimeRange( Time start, Time end )
    {
      this.start = start;
      this.end = end;
    }
    /// <summary>
    /// Gets or Sets the Start Time value of this range.
    /// </summary>
    /// <param>
    ///	Sets the Start Time value of this range.
    /// </param>
    /// <returns>
    ///	Gets the Start Time value of this range.
    /// </returns>
    /// <exception cref="System.ApplicationException">
    /// This will throw an exception if the start time is greater then or equal to the end time.
    /// </exception>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Time" ),
    DefaultValue( null ),
    Description( "Gets or Sets the Start Time value of this range." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public Time Start
    {
      get
      {
        return this.start;
      }
      set
      {
        if ( end.MilitaryTime > -1 && value.MilitaryTime < end.MilitaryTime )
        {
          this.start = value;
        }
        else
        {
          throw new System.ApplicationException( "The Start value may not be greater then or equal to the End value." );
        }
      }
    }
    /// <summary>
    /// Gets or Sets the End Time value of this range.
    /// </summary>
    /// <param>
    ///	Sets the End Time value of this range.
    /// </param>
    /// <returns>
    ///	Gets the End Time value of this range.
    /// </returns>
    /// <exception cref="System.ApplicationException">
    /// This will throw an exception if the end time is less then or equal to the start time.
    /// </exception>
    [
    Bindable( false ),
    Browsable( true ),
    Category( "Time" ),
    DefaultValue( null ),
    Description( "Gets or Sets the End Time value of this range." ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
    ]
    public Time End
    {
      get
      {
        return this.end;
      }
      set
      {

        if ( start.MilitaryTime > -1 && value.MilitaryTime > start.MilitaryTime )
        {
          this.end = value;
        }
        else
        {
          throw new System.ApplicationException( "The End value may not be less then or equal to the Start value." );
        }
      }
    }
  }
  /// <summary>
  ///   A collection that stores <see cref='TimeRange'/> objects.
  /// </summary>
  [Serializable()]
  public class TimeRangeCollection: CollectionBase
  {
    /// <summary>
    /// Initializes a new instance of <see cref='TimeRangeCollection'/>.
    /// </summary>
    public TimeRangeCollection()
    {
    }
    /// <summary>
    ///   Initializes a new instance of <see cref='TimeRangeCollection'/> based on another <see cref='TimeRangeCollection'/>.
    /// </summary>
    /// <param name='val'>
    ///   A <see cref='TimeRangeCollection'/> from which the contents are copied
    /// </param>
    public TimeRangeCollection( TimeRangeCollection val )
    {
      this.AddRange( val );
    }
    /// <summary>
    ///   Initializes a new instance of <see cref='TimeRangeCollection'/> containing any array of <see cref='TimeRange'/> objects.
    /// </summary>
    /// <param name='val'>
    ///       A array of <see cref='TimeRange'/> objects with which to initialize the collection
    /// </param>
    public TimeRangeCollection( TimeRange[] val )
    {
      this.AddRange( val );
    }
    /// <summary>
    ///   Represents the entry at the specified index of the <see cref='TimeRange'/>.
    /// </summary>
    /// <param name='index'>The zero-based index of the entry to locate in the collection.</param>
    /// <value>The entry at the specified index of the collection.</value>
    /// <exception cref='ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
    public TimeRange this[ int index ]
    {
      get
      {
        return ( (TimeRange)( List[ index ] ) );
      }
      set
      {
        List[ index ] = value;
      }
    }
    /// <summary>
    ///   Adds a <see cref='TimeRange'/> with the specified value to the
    ///   <see cref='TimeRangeCollection'/>.
    /// </summary>
    /// <param name='val'>The <see cref='TimeRange'/> to add.</param>
    /// <returns>The index at which the new element was inserted.</returns>
    public int Add( TimeRange val )
    {
      return List.Add( val );
    }
    /// <summary>
    ///   Copies the elements of an array to the end of the <see cref='TimeRangeCollection'/>.
    /// </summary>
    /// <param name='val'>
    ///    An array of my_Type <see cref='TimeRange'/> containing the objects to add to the collection.
    /// </param>
    /// <seealso cref='TimeRangeCollection.Add'/>
    public void AddRange( TimeRange[] val )
    {
      for ( int i = 0; i < val.Length; i++ )
      {
        this.Add( val[ i ] );
      }
    }
    /// <summary>
    ///   Adds the contents of another <see cref='TimeRangeCollection'/> to the end of the collection.
    /// </summary>
    /// <param name='val'>
    ///    A <see cref='TimeRangeCollection'/> containing the objects to add to the collection.
    /// </param>
    /// <seealso cref='TimeRangeCollection.Add'/>
    public void AddRange( TimeRangeCollection val )
    {
      for ( int i = 0; i < val.Count; i++ )
      {
        this.Add( val[ i ] );
      }
    }
    /// <summary>
    ///   Gets a value indicating whether the
    ///    <see cref='TimeRangeCollection'/> contains the specified <see cref='TimeRange'/>.
    /// </summary>
    /// <param name='val'>The <see cref='TimeRange'/> to locate.</param>
    /// <returns>
    /// <see langword='true'/> if the <see cref='TimeRange'/> is contained in the collection;
    ///   otherwise, <see langword='false'/>.
    /// </returns>
    /// <seealso cref='TimeRangeCollection.IndexOf'/>
    public bool Contains( TimeRange val )
    {
      return List.Contains( val );
    }
    /// <summary>
    /// Copies the <see cref="TimeRangeCollection" /> values to a one-dimensional <see cref="Array" /> instance at the
    /// specified index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the values copied from <see cref="TimeRangeCollection" />.</param>
    /// <param name="index">The index in <paramref name="array" /> where copying begins.</param>
    /// <exception cref="ArgumentException"><para>
    ///   <paramref name="array" /> is multidimensional.</para>
    /// <para>-or-</para>
    /// <para>The number of elements in the <see cref="TimeRangeCollection" /> is greater than
    /// the available space between <paramref name="index" /> and the end of
    /// <paramref name="array" />.</para></exception>
    /// <exception cref="ArgumentNullException"><paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than <paramref name="array" />'s low-bound.</exception>
    /// <seealso cref="Array" />
    public void CopyTo( TimeRange[] array, int index )
    {
      List.CopyTo( array, index );
    }
    /// <summary>
    ///    Returns the index of a <see cref='TimeRange'/> in
    ///       the <see cref='TimeRangeCollection'/>.
    /// </summary>
    /// <param name='val'>The <see cref='TimeRange'/> to locate.</param>
    /// <returns>
    ///   The index of the <see cref='TimeRange'/> of <paramref name='val'/> in the
    ///   <see cref='TimeRangeCollection'/>, if found; otherwise, -1.
    /// </returns>
    /// <seealso cref='TimeRangeCollection.Contains'/>
    public int IndexOf( TimeRange val )
    {
      return List.IndexOf( val );
    }
    /// <summary>
    ///   Inserts a <see cref='TimeRange'/> into the <see cref='TimeRangeCollection'/> at the specified index.
    /// </summary>
    /// <param name='index'>The zero-based index where <paramref name='val'/> should be inserted.</param>
    /// <param name='val'>The <see cref='TimeRange'/> to insert.</param>
    /// <seealso cref='TimeRangeCollection.Add'/>
    public void Insert( int index, TimeRange val )
    {
      List.Insert( index, val );
    }
    /// <summary>
    ///  Returns an enumerator that can iterate through the <see cref='TimeRangeCollection'/>.
    /// </summary>
    /// <seealso cref='IEnumerator'/>
    public new TimeRangeEnumerator GetEnumerator()
    {
      return new TimeRangeEnumerator( this );
    }
    /// <summary>
    ///   Removes a specific <see cref='TimeRange'/> from the <see cref='TimeRangeCollection'/>.
    /// </summary>
    /// <param name='val'>The <see cref='TimeRange'/> to remove from the <see cref='TimeRangeCollection'/>.</param>
    /// <exception cref='ArgumentException'><paramref name='val'/> is not found in the Collection.</exception>
    public void Remove( TimeRange val )
    {
      List.Remove( val );
    }
    /// <summary>
    ///   Enumerator that can iterate through a TimeRangeCollection.
    /// </summary>
    /// <seealso cref='IEnumerator'/>
    /// <seealso cref='TimeRangeCollection'/>
    /// <seealso cref='TimeRange'/>
    public class TimeRangeEnumerator: IEnumerator
    {
      IEnumerator baseEnumerator;
      IEnumerable temp;
      /// <summary>
      ///   Initializes a new instance of <see cref='TimeRangeEnumerator'/>.
      /// </summary>
      public TimeRangeEnumerator( TimeRangeCollection mappings )
      {
        this.temp = ( (IEnumerable)( mappings ) );
        this.baseEnumerator = temp.GetEnumerator();
      }
      /// <summary>
      ///   Gets the current <see cref='TimeRange'/> in the <seealso cref='TimeRangeCollection'/>.
      /// </summary>
      public TimeRange Current
      {
        get
        {
          return ( (TimeRange)( baseEnumerator.Current ) );
        }
      }
      object IEnumerator.Current
      {
        get
        {
          return baseEnumerator.Current;
        }
      }
      /// <summary>
      ///   Advances the enumerator to the next <see cref='TimeRange'/> of the <see cref='TimeRangeCollection'/>.
      /// </summary>
      public bool MoveNext()
      {
        return baseEnumerator.MoveNext();
      }
      /// <summary>
      ///   Sets the enumerator to its initial position, which is before the first element in the <see cref='TimeRangeCollection'/>.
      /// </summary>
      public void Reset()
      {
        baseEnumerator.Reset();
      }
    }
  }
}
