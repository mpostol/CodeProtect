using CAS.Lib.CodeProtect.EnvironmentAccess;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace CAS.CodeProtect.UnitTests
{
  [TestClass]
  public class ManifestManagementUnitTest
  {
    [TestMethod]
    [TestCategory("CAS Core")]
    public void ManifestFromAssemblyManagementWriteReadTestMethod()
    {
      System.IO.FileInfo _fi = new System.IO.FileInfo(FileNames.ManifestFileName);
      Assembly _thisAssembly = Assembly.GetExecutingAssembly();
      ManifestManagement.WriteDeployManifest(_thisAssembly, CommonDefinitions.AssemblyProduct);
      Assert.IsTrue(_fi.Exists);
      DeployManifest _manifest = ManifestManagement.ReadDeployManifest();
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
    }
    [TestMethod]
    [TestCategory("CAS Core")]
    public void ManifestFromPropertiesManagementWriteReadTestMethod()
    {
      System.IO.FileInfo _fi = new System.IO.FileInfo(FileNames.ManifestFileName);
      AssemblyName _name = new AssemblyName()
      {
        ContentType = AssemblyContentType.Default,
        CodeBase = "CodeBase",
        CultureInfo = System.Globalization.CultureInfo.InvariantCulture,
        CultureName = System.Globalization.CultureInfo.InvariantCulture.Name,
        Flags = AssemblyNameFlags.None,
        Name = "Name",
        HashAlgorithm = System.Configuration.Assemblies.AssemblyHashAlgorithm.None,
        KeyPair = null,
        ProcessorArchitecture = ProcessorArchitecture.MSIL,
        Version = new Version(1, 2, 3),
        VersionCompatibility = System.Configuration.Assemblies.AssemblyVersionCompatibility.SameProcess,
      };
      _name.SetPublicKeyToken(new byte[] { 0x88, 0x32, 0xff, 0x1a, 0x67, 0xea, 0x61, 0xa3 });
      ManifestManagement.WriteDeployManifest(_name, ManifestManagement.ProductType.SingleUser, "publisher", new System.Uri("http://www.commsvr.eu/"), new System.Uri("http://www.commsvr.eu/"), "appData");
      Assert.IsTrue(_fi.Exists);
      DeployManifest _manifest = ManifestManagement.ReadDeployManifest();
      Assert.IsNotNull(_manifest);

      Assert.IsNotNull(_manifest.AssemblyIdentity);
      Assert.IsNotNull(_manifest.AssemblyReferences);
      Assert.IsFalse(_manifest.CreateDesktopShortcut);
      Assert.AreEqual<string>("http://www.commsvr.eu/", _manifest.DeploymentUrl);
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.Description));
      Assert.IsFalse(_manifest.DisallowUrlActivation);
      Assert.IsNull(_manifest.EntryPoint);
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.ErrorReportUrl));
      Assert.IsNotNull(_manifest.FileReferences);
      Assert.IsNull(_manifest.InputStream);
      Assert.IsTrue(_manifest.Install);
      Assert.IsFalse(_manifest.MapFileExtensions);
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.MinimumRequiredVersion));
      Assert.IsNotNull(_manifest.OutputMessages);
      Assert.AreEqual<string>("Name", _manifest.Product);

      Assert.AreEqual<string>("publisher", _manifest.Publisher); //Only in test environment 
      Assert.IsFalse(_manifest.ReadOnly);
      Assert.IsTrue(_manifest.SourcePath.Contains("CAS.Product.xml"));
      Assert.IsTrue(String.IsNullOrEmpty(_manifest.SuiteName));
      Assert.AreEqual<string>("http://www.commsvr.eu/", _manifest.SupportUrl);
      Assert.IsFalse(_manifest.TrustUrlParameters);
      Assert.IsFalse(_manifest.UpdateEnabled);
      Assert.AreEqual<int>(1, _manifest.UpdateInterval);
      Assert.AreEqual<UpdateMode>( UpdateMode.Background, _manifest.UpdateMode);
      Assert.AreEqual<UpdateMode>(UpdateMode.Background, _manifest.UpdateMode);
      Assert.AreEqual<UpdateUnit>(UpdateUnit.Days, _manifest.UpdateUnit);

      AssemblyIdentity _id = _manifest.AssemblyIdentity;
      Assert.IsNotNull(_id);
      Assert.AreNotEqual<string>(new System.Version(1, 2, 4).ToString(), _id.Version);
      Assert.AreEqual<string>(new System.Version(1, 2, 3).ToString(), _id.Version);
    }
  }
}
