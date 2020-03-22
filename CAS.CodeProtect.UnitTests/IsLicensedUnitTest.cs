using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAOOI.CodeProtect;
using UAOOI.CodeProtect.LicenseDsc;
using System.Collections.Generic;

namespace UAOOI.CodeProtect
{
  [TestClass]
  public class IsLicensedUnitTest
  {
    [TestMethod]
    [ExpectedException(typeof(ApplicationException))]
    public void NotSealedClassTest()
    {
      NotSealedClass _newNotSealedClass = new NotSealedClass();
    }
    [TestMethod]
    public void NoAttributesClassTest()
    {
      NoAttributesClass _NoAttributesClass = new NoAttributesClass();
      Assert.IsFalse(_NoAttributesClass.TraceCurrentLicenseCalled);
      Assert.IsFalse(_NoAttributesClass.TraceFailureReasonCalled);
      Assert.IsTrue(_NoAttributesClass.TraceNoLicenseFileCalled);
      Assert.IsFalse(_NoAttributesClass.TraceWarningCalled);
      Assert.IsFalse(_NoAttributesClass.Licensed);
      Assert.IsFalse(_NoAttributesClass.TraceCurrentLicenseCalled);
      Assert.IsFalse(_NoAttributesClass.TraceFailureReasonCalled);
      Assert.IsTrue(_NoAttributesClass.TraceNoLicenseFileCalled);
      Assert.IsFalse(_NoAttributesClass.TraceWarningCalled);
    }
    private class NotSealedClass : IsLicensed<NotSealedClass> { }
    private sealed class NoAttributesClass : IsLicensed<NoAttributesClass>
    {
      internal bool TraceCurrentLicenseCalled = false;
      internal bool TraceFailureReasonCalled = false;
      internal bool TraceNoLicenseFileCalled = false;
      internal bool TraceWarningCalled = false;

      protected override void TraceCurrentLicense(LicenseFile license)
      {
        base.TraceCurrentLicense(license);
        TraceCurrentLicenseCalled = true;
      }
      protected override void TraceFailureReason(string reason)
      {
        base.TraceFailureReason(reason);
        TraceFailureReasonCalled = true;
      }
      protected override void TraceNoLicenseFile(string reason)
      {
        base.TraceNoLicenseFile(reason);
        TraceNoLicenseFileCalled = true;
      }
      protected override void TraceWarning(List<string> warning)
      {
        base.TraceWarning(warning);
        TraceWarningCalled = true;
      }
    }
  }
}
