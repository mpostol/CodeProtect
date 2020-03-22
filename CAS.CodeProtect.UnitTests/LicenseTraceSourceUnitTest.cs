
using UAOOI.CodeProtect.Instrumentation;
using UAOOI.CodeProtect.EnvironmentAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UAOOI.CodeProtect
{

  [TestClass]
  public class LicenseTraceSourceUnitTest
  {

    [TestMethod]
    public void AfterCreationTest()
    {

      LicenseTraceSource _tracer = LicenseTraceSource.This;
      Assert.IsNotNull(_tracer);

      //Listeners
      Assert.AreEqual(1, _tracer.Listeners.Count);
      Dictionary<string, TraceListener> _listeners = _tracer.Listeners.Cast<TraceListener>().ToDictionary<TraceListener, string>(x => x.Name);
      Assert.IsTrue(_listeners.ContainsKey("LogFile"));
      TraceListener _listener = _listeners["LogFile"];
      Assert.IsNotNull(_listener);
      Assert.IsInstanceOfType(_listener, typeof(AdvancedDelimitedListTraceListener));
      AdvancedDelimitedListTraceListener _advancedListener = _listener as AdvancedDelimitedListTraceListener;
      Assert.AreEqual<string>(ExpectedFileName, _advancedListener.GetFileName());

      //Filter
      Assert.IsNotNull(_advancedListener.Filter);
      Assert.IsInstanceOfType(_advancedListener.Filter, typeof(EventTypeFilter));
      EventTypeFilter _eventTypeFilter = _advancedListener.Filter as EventTypeFilter;
      Assert.AreEqual(SourceLevels.All, _eventTypeFilter.EventType);

      //Test Switch
      Assert.IsNotNull(_tracer.Switch);
      Assert.AreEqual<string>("UAOOI.CodeProtect.TraceSource.Switch", _tracer.Switch.DisplayName);
      Assert.AreEqual<SourceLevels>(SourceLevels.All, _tracer.Switch.Level);

      //Trace
      Assert.IsFalse(Trace.Listeners.Cast<TraceListener>().Where<TraceListener>(x => x.Name == "LogFile").Any<TraceListener>());

    }
    [TestMethod]
    public void LogFileTest()
    {
      LicenseTraceSource _tracer = LicenseTraceSource.This;
      Assert.IsNotNull(_tracer);
      _tracer.TraceEvent(TraceEventType.Error, 0, "Test message");
      _tracer.Flush();
      FileInfo _logFile = new FileInfo( ExpectedFileName);
      Assert.IsTrue(_logFile.Exists);
      Assert.IsTrue(_logFile.Length > 10);
    }
    private static string ExpectedFileName
    {
      get
      {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UAOOI.RealTimeUnitTests.log");
      }
    }

  }
}
