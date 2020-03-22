//<summary>
//  Title   : Provides access to a set of management information and management events about the system, 
//            devices, and applications instrumented to the Windows Management Instrumentation (WMI) infrastructure.
//  System  : Microsoft Visual C# .NET 2005
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//  History :
//    MPostol - 20-11-2006 created
//    <Author> - <date>:
//    <description>
//
//  Copyright (C)2006, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto:techsupp@UAOOI.com.pl
//  http:\\www.UAOOI.eu
//</summary>
using System.Management;

namespace UAOOI.CodeProtect.EnvironmentAccess
{
  class WMIAccess
  {
    /// <summary>
    /// Gets unique identifier of a processor on the system. 
    /// </summary>
    internal static string GetProcessorId
    {
      get
      {
        string strProcessorId = "";
        SelectQuery query = new SelectQuery( "Win32_processor" );
        ManagementObjectSearcher search = new ManagementObjectSearcher( query );
        foreach ( ManagementObject info in search.Get() )
          strProcessorId = info[ "processorId" ].ToString();
        return strProcessorId;
      }
    }
    /// <summary>
    /// Gets unique identifier of this motherboard. 
    /// </summary>
    internal static string GetMotherboardDeviceID
    {
      get
      {
        string strProcessorId = string.Empty;
        SelectQuery query = new SelectQuery( "Win32_MotherboardDevice" );
        ManagementObjectSearcher search = new ManagementObjectSearcher( query );
        foreach ( ManagementObject info in search.Get() )
          strProcessorId = info[ "DeviceID" ].ToString() + ": " + info[ "Caption" ];
        return strProcessorId;
      }
    }
  }
}
