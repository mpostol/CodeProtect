
using UAOOI.CodeProtect;
using UAOOI.CodeProtect.EnvironmentAccess;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;

namespace UAOOI.CodeProtect
{
  [TestClass]
  public class ManifestManagementUnitTest
  {
    [TestMethod]
    public void ManifestFromAssemblyManagementWriteReadTestMethod()
    {
      FileInfo _fi = new FileInfo(FileNames.ManifestFileName);
      Assert.IsFalse(_fi.Exists);
      Assembly _thisAssembly = Assembly.GetExecutingAssembly();
      ManifestManagement.WriteDeployManifest(_thisAssembly, CommonDefinitions.AssemblyProduct);
      _fi.Refresh();
      Assert.IsTrue(_fi.Exists);
      DeployManifest _manifest = ManifestManagement.ReadDeployManifest();
      ManifestManagement.DeleteDeployManifest();
      _fi.Refresh();
      Assert.IsFalse(_fi.Exists);
      //test the content
      Assert.IsNotNull(_manifest);
      Assert.IsTrue(_manifest.Install);
      Assert.AreEqual<string>("CodeProtectTests", _manifest.Product);
      Assert.AreEqual<string>("CAS", _manifest.Publisher); //Only in test environment 
      Assert.AreEqual<string>("http://www.commsvr.eu/", _manifest.SupportUrl);
      Assert.AreEqual<string>("http://www.commsvr.eu/", _manifest.DeploymentUrl);
      AssemblyIdentity _id = _manifest.AssemblyIdentity;
      Assert.IsNotNull(_id);
      AssemblyName _thisName = _thisAssembly.GetName();
      Assert.AreEqual<string>(_thisName.Version.ToString(), _id.Version);
      //FileReferences
      Assert.IsNotNull(_manifest.FileReferences);
      Assert.AreEqual<int>(1, _manifest.FileReferences.Count);
      FileReference _FileReference = _manifest.FileReferences[0];
      Assert.AreEqual<string>(FileNames.TargetDir, _FileReference.TargetPath);
      Assert.AreEqual<string>("license", _FileReference.Group);
      Assert.IsFalse(_FileReference.IsOptional);
      Assert.IsTrue(_FileReference.IsDataFile);
    }
    [TestMethod]
    public void ManifestFromPropertiesManagementWriteReadTestMethod()
    {
      Assert.Inconclusive("SystemFakes doesn't work");
      FileInfo _fi = new FileInfo(FileNames.ManifestFileName);
      Assert.IsFalse(_fi.Exists);
      InstallContext _context = new InstallContext();
      _context.Parameters.Add(InstallContextNames.Productname, nameof(InstallContextNames.Productname));
      _context.Parameters.Add(InstallContextNames.Version, new Version(1, 1, 1, 1).ToString());
      _context.Parameters.Add(InstallContextNames.Allusers, $"{true}");
      _context.Parameters.Add(InstallContextNames.Manufacturer, nameof(InstallContextNames.Manufacturer));
      _context.Parameters.Add(InstallContextNames.Arphelplink, new Uri(@"http://www.contoso.com/").ToString());
      string _applicationDataPath = FileNames.ConstructApplicationDataFolder(nameof(InstallContextNames.Manufacturer), nameof(InstallContextNames.Productname));
      DeployManifest _manifest = null;
      //using (ShimsContext.Create())
      //{
      //  bool _existCalled = false;
      //  string _applicationDataPathFullName = String.Empty;
      //  ShimDirectoryInfo.AllInstances.ExistsGet = x =>
      //  {
      //    _existCalled = true;
      //    _applicationDataPathFullName = x.FullName;
      //    return true;
      //  };
      //  ManifestManagement.ChangeSetModifyRights(_applicationData => Assert.AreEqual<string>(_applicationDataPath, _applicationData));
      //  ManifestManagement.WriteDeployManifest(_context);

      //  Assert.IsTrue(_existCalled);
      //  Assert.AreEqual<string>(_applicationDataPath, _applicationDataPathFullName);
      //  _fi.Refresh();
      //  Assert.IsTrue(_fi.Exists);
      //  _manifest = ManifestManagement.ReadDeployManifest();
      //  ManifestManagement.DeleteDeployManifest();
      //  _fi.Refresh();
      //  Assert.IsFalse(_fi.Exists);
      //}
      Assert.IsNotNull(_manifest);
      Assert.IsNotNull(_manifest.AssemblyIdentity);
      Assert.IsNotNull(_manifest.AssemblyReferences);
      Assert.IsFalse(_manifest.CreateDesktopShortcut);
      Assert.AreEqual<string>("http://www.contoso.com/", _manifest.DeploymentUrl);
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.Description));
      Assert.IsFalse(_manifest.DisallowUrlActivation);
      Assert.IsNull(_manifest.EntryPoint);
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.ErrorReportUrl));
      //FileReferences
      Assert.IsNotNull(_manifest.FileReferences);
      Assert.AreEqual<int>(1, _manifest.FileReferences.Count);
      FileReference _FileReference = _manifest.FileReferences[0];
      Assert.AreEqual<string>(FileNames.ConstructApplicationDataFolder(_manifest.Publisher, _manifest.Product), _FileReference.TargetPath);
      Assert.AreEqual<string>("license", _FileReference.Group);
      Assert.IsFalse(_FileReference.IsOptional);
      Assert.IsTrue(_FileReference.IsDataFile);

      Assert.IsNull(_manifest.InputStream);
      Assert.IsTrue(_manifest.Install);
      Assert.IsFalse(_manifest.MapFileExtensions);
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.MinimumRequiredVersion));
      Assert.IsNotNull(_manifest.OutputMessages);
      Assert.AreEqual<string>(nameof(InstallContextNames.Productname), _manifest.Product);
      Assert.AreEqual<string>(nameof(InstallContextNames.Manufacturer), _manifest.Publisher);
      Assert.IsFalse(_manifest.ReadOnly);
      Assert.AreEqual(Path.GetFileName(_manifest.SourcePath), "UAOOI.Product.xml");
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.SuiteName));
      Assert.AreEqual<string>("http://www.contoso.com/", _manifest.SupportUrl);
      Assert.IsFalse(_manifest.TrustUrlParameters);
      Assert.IsFalse(_manifest.UpdateEnabled);
      Assert.AreEqual<int>(1, _manifest.UpdateInterval);
      Assert.AreEqual<UpdateMode>(UpdateMode.Background, _manifest.UpdateMode);
      Assert.AreEqual<UpdateMode>(UpdateMode.Background, _manifest.UpdateMode);
      Assert.AreEqual<UpdateUnit>(UpdateUnit.Days, _manifest.UpdateUnit);

      AssemblyIdentity _id = _manifest.AssemblyIdentity;
      Assert.IsNotNull(_id);
      Assert.AreNotEqual<string>(new System.Version(1, 1, 1, 0).ToString(), _id.Version);
      Assert.AreEqual<string>(new System.Version(1, 1, 1, 1).ToString(), _id.Version);
      Assert.AreNotEqual<string>(new System.Version(1, 1, 1, 2).ToString(), _id.Version);
    }
  }
}
