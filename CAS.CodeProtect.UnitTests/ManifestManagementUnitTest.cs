using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS.Lib.CodeProtect.EnvironmentAccess;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace CAS.Lib.CodeProtect.Tests
{
  [TestClass]
  public class ManifestManagementUnitTest
  {
    [TestMethod]
    [TestCategory("CAS Core")]
    public void FileNamesManifestFileNameTestMethod()
    {
      Debug.WriteLine(String.Format("Manifest File Name {0}", FileNames.ManifestFileName));
      Assert.IsTrue(FileNames.ManifestFileName.Contains(@"PR36-CAS_MAIN_CORE_PCKG\UT\CodeProtect\CodeProtectTests\bin\Debug"));
    }
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
