
using CAS.Lib.CodeProtect;
using CAS.Lib.CodeProtect.EnvironmentAccess;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace CAS.CodeProtect.UnitTests
{
  [TestClass]
  public class LibInstallerUnitTest
  {
    [TestMethod]
    public void InstallLicenseFromContainerDefaultParametersTest()
    {
      try
      {
        LibInstaller.InstallLicense(true);
        Assert.Fail();
      }
      catch (Exception ex)
      {
        DeployManifest _manifest = FileNames.ProductManifest();
        Assert.IsNotNull(_manifest);
        Assert.AreEqual<string>("CAS.CodeProtect.UnitTests", _manifest.Product);
        Assert.IsFalse(ex.Message.Contains("CAS.CodeProtect.UnitTests"));
        Assert.IsTrue(ex.Message.Contains("CodeProtect.UnitTests"));
        ManifestManagement.DeleteDeployManifest();
      }
    }
    [TestMethod]
    public void InstallLicenseFromContainerAllParametersTest()
    {
      try
      {
        LibInstaller.InstallLicense("user", "company", "email", true, "", "", Assembly.GetExecutingAssembly());
        Assert.Fail();
      }
      catch (Exception ex)
      {
        DeployManifest _manifest = FileNames.ProductManifest();
        Assert.IsNotNull(_manifest);
        Assert.AreEqual<string>("CAS.CodeProtect.UnitTests", _manifest.Product);
        Assert.IsFalse(ex.Message.Contains("CAS.CodeProtect.UnitTests"));
        Assert.IsTrue(ex.Message.Contains("CodeProtect.UnitTests"));
        ManifestManagement.DeleteDeployManifest();
      }

    }
  }
}
