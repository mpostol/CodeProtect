using CAS.Lib.CodeProtect.EnvironmentAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Reflection;

namespace CAS.CodeProtect.UnitTests.Instrumentation
{
  internal static class Extensions
  {
    internal static string GetFileName(this AdvancedDelimitedListTraceListener _listener)
    {
      FieldInfo fi = typeof(TextWriterTraceListener).GetField("fileName", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.IsNotNull(fi);
      return (string)fi.GetValue(_listener);
    }

  }
}
