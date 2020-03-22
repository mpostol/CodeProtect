//<summary>
//  Title   : UAOOI.CodeProtect.LicenseDsc.LicenseFileException: Represents errors that occur during application execution.
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2010, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@UAOOI.eu
//  http://www.UAOOI.eu
//</summary>

using System;

namespace UAOOI.CodeProtect.LicenseDsc
{
  /// <summary>
  /// Represents errors that occur during application execution. 
  /// </summary>
  [Serializable]
  public class LicenseFileException: ApplicationException
  {
    /// <summary>
    /// Initializes a new instance of the LicenseFileException class with a specified error message. 
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    internal LicenseFileException( string message ) : base( message ) { }
    /// <summary>
    /// Initializes a new instance of the Exception class with a specified error message and 
    /// a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference 
    /// if no inner exception is specified. 
    /// </param>
    internal LicenseFileException( string message, Exception innerException ) : base( message, innerException ) { }
    /// <summary>
    /// Traces the inner exceptions - adds all the messages from inner exceptions to the end of the <paramref name="message"/>.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="message">The message.</param>
    /// <returns>The expanded message.</returns>
    public static string TraceInnerExceptions( Exception ex, string message )
    {
      Exception cex = ex;
      while ( cex != null )
      {
        message += " " + cex.Message;
        cex = cex.InnerException;
      }
      return message;
    }

  }
}
