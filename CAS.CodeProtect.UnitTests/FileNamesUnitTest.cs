using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAOOI.CodeProtect.EnvironmentAccess;
using System.IO;
using System;
using System.Reflection;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace UAOOI.CodeProtect
{
  [TestClass]
  public class FileNamesUnitTest
  {
    [TestMethod]
    public void PropertiesTest()
    {
      Assert.AreEqual<string>(FileNames.TargetDir, FileNames.ApplicationDataPath);
      Assert.IsTrue(FileNames.ApplicationDataPath.LastIndexOf(Path.PathSeparator) < FileNames.ApplicationDataPath.Length);
      Assert.AreEqual<string>("UAOOI.Product.xml", Path.GetFileName(FileNames.ManifestFileName));
      Assert.AreEqual<string>("lic", Path.GetFileName(FileNames.LicExtension));
      Assert.AreEqual<string>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileNames.TargetDir);
      Assert.AreEqual<string>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileNames.LicenseFileName), FileNames.LicenseFilePath);
    }
    [TestMethod]
    public void ConstructApplicationDataFolderTest()
    {
      string _applicationDataFolder = FileNames.ConstructApplicationDataFolder("Manufacturer", "ProductName");
      string _localPath = Path.Combine("Manufacturer", "ProductName");
      Assert.AreEqual<string>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), _localPath) + Path.DirectorySeparatorChar, _applicationDataFolder);
    }
    [TestMethod]
    [DeploymentItem(@"TestingData\", @"TestingData\")]
    public void ProductManifestTest()
    {
      FileInfo _fi = new FileInfo(FileNames.ManifestFileName);
      Assert.IsFalse(_fi.Exists, FileNames.ManifestFileName);
      FileNames.UnloadProductManifest();
      DeployManifest _manifest = FileNames.ProductManifest();
      Assert.IsNull(_manifest);
      FileInfo _testManifest = new FileInfo(Path.Combine(@"TestingData\", "UAOOI.Product.xml"));
      Assert.IsTrue(_testManifest.Exists);
      _testManifest.CopyTo(FileNames.ManifestFileName);
      FileInfo _manifestFileInfo = new FileInfo(FileNames.ManifestFileName);
      Assert.IsTrue(_manifestFileInfo.Exists);
      _manifest = FileNames.ProductManifest();
      Assert.IsNotNull(_manifest);
      _manifestFileInfo.Delete();
      _manifestFileInfo.Refresh();
      Assert.IsFalse(_manifestFileInfo.Exists);
      _manifest = FileNames.ProductManifest();
      Assert.IsNotNull(_manifest);
      Assert.AreEqual(@"TestApplicationDataPath\0D42ACCD-BBE0-4F79-9307-B746E6E09937", FileNames.ApplicationDataPath);
      FileNames.UnloadProductManifest();
      _manifest = FileNames.ProductManifest();
      Assert.IsNull(_manifest);
    }
  }
}
