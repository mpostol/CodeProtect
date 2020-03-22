
using UAOOI.CodeProtect;
using UAOOI.CodeProtect.EnvironmentAccess;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace UAOOI.CodeProtect
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
        Assert.AreEqual<string>("UAOOI.CodeProtect", _manifest.Product);
        Assert.IsFalse(ex.Message.Contains("UAOOI.CodeProtect"));
        Assert.IsTrue(ex.Message.Contains("CodeProtect"));
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
        Assert.AreEqual<string>("UAOOI.CodeProtect", _manifest.Product);
        Assert.IsFalse(ex.Message.Contains("UAOOI.CodeProtect"));
        Assert.IsTrue(ex.Message.Contains("CodeProtect"));
        ManifestManagement.DeleteDeployManifest();
      }

    }
  }
}
