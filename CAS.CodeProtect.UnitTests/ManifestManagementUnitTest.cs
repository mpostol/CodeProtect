using CAS.Lib.CodeProtect.EnvironmentAccess;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace CAS.Lib.CodeProtect.Tests
{
  [TestClass]
  public class ManifestManagementUnitTest
  {
    [TestMethod]
    [TestCategory("CAS Core")]
    public void ManifestManagementWriteReadTestMethod()
    {
      System.IO.FileInfo _fi = new System.IO.FileInfo(FileNames.ManifestFileName);
      ManifestManagement.WriteDeployManifest(Assembly.GetCallingAssembly(), CommonDefinitions.AssemblyProduct);
      Assert.IsTrue(_fi.Exists);
      DeployManifest _manifest = ManifestManagement.ReadDeployManifest();
      Assert.IsNotNull(_manifest);
      Assert.IsTrue(_manifest.Install);
      Assert.AreEqual<string>("CodeProtectTests", _manifest.Product);
      Assert.AreEqual<string>("Microsoft Corporation", _manifest.Publisher); //Only in test environment 
      Assert.AreEqual<string>("http://www.commsvr.eu/", _manifest.SupportUrl);
      Assert.AreEqual<string>("http://www.commsvr.eu/", _manifest.DeploymentUrl);
    }
  }
}
