
using CAS.Lib.CodeProtect.EnvironmentAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CAS.CodeProtect.UnitTests
{
  [TestClass]
  public class UnlockKeyAssemblyContainerUnitTest
  {
    [TestMethod]
    public void GetManifestResourcePath4NotExistingCodeTest()
    {
      UnlockKeyAssemblyContainer _licensesContainer = new UnlockKeyAssemblyContainer();
      string _path = _licensesContainer.GetManifestResourcePath("Random text");
      Assert.IsTrue(String.IsNullOrEmpty(_path));
    }
  }
}
