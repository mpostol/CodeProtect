using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CAS.Lib.CodeProtect.LicenseDsc;
using System.Reflection;

namespace CAS.CodeProtect.UnitTests
{
  [TestClass]
  public class ProductUnitTest
  {
    [TestMethod]
    public void CreatorTestMethod1()
    {
      Assembly _expectedProduct = Assembly.GetExecutingAssembly();
      AssemblyName _expectedName = _expectedProduct.GetName();
      Product _newProduct = new Product(_expectedProduct);
      Assert.IsTrue(String.IsNullOrEmpty(_newProduct.Description));
      Assert.IsTrue(String.IsNullOrEmpty(_newProduct.Developer));
      Assert.IsTrue(String.IsNullOrEmpty(_newProduct.FilePath));
      Assert.AreEqual<string>(_expectedName.FullName, _newProduct.FullName);
      Assert.IsFalse(_newProduct.IsDirty);
      Assert.IsFalse(_newProduct.IsDirty);
      Assert.IsFalse(_newProduct.IsLicensed);
      Assert.AreEqual<string>("CAS.CodeProtect.UnitTests.lic", _newProduct.LicFileName);
      Assert.AreEqual<string>(_expectedName.Name, _newProduct.ShortName);
      Assert.AreEqual<string>(_expectedName.Version.ToString(), _newProduct.Version);
    }
    [TestMethod]
    [ExpectedException(typeof(LicenseFileException))]
    public void VersionExceptionLessTestMethod()
    {
      Assembly _expectedProduct = Assembly.GetExecutingAssembly();
      Product _existingProduct = new Product(_expectedProduct);
      Product _upgradeProduct = new Product(_expectedProduct);
      _existingProduct.Version = new Version(2, 0, 12).ToString();
      _upgradeProduct.Version = new Version(2, 0, 11).ToString();
      _existingProduct.UpgardeData(_upgradeProduct);
    }
    [TestMethod]
    public void VersionEqualUpgradeTestMethod()
    {
      Assembly _expectedProduct = Assembly.GetExecutingAssembly();
      Product _existingProduct = new Product(_expectedProduct);
      Product _upgradeProduct = new Product(_expectedProduct);
      _existingProduct.Version = new Version(2, 0, 12).ToString();
      _upgradeProduct.Version = new Version(2, 0, 12).ToString();
      _existingProduct.UpgardeData(_upgradeProduct);
      Assert.AreEqual<string>(_existingProduct.Version, _upgradeProduct.Version);
    }
    [TestMethod]
    public void VersionGreaderUpgradeTestMethod()
    {
      Assembly _expectedProduct = Assembly.GetExecutingAssembly();
      Product _existingProduct = new Product(_expectedProduct);
      Product _upgradeProduct = new Product(_expectedProduct);
      _existingProduct.Version = new Version(2, 0, 12).ToString();
      _upgradeProduct.Version = new Version(2, 0, 13).ToString();
      _existingProduct.UpgardeData(_upgradeProduct);
      Assert.AreNotEqual<string>(_existingProduct.Version, _upgradeProduct.Version);
    }
    [TestMethod]
    public void SameNamesExceptionTestMethod()
    {
      Assembly _expectedProduct = Assembly.GetExecutingAssembly();
      Product _existingProduct = new Product(_expectedProduct);
      Product _upgradeProduct = new Product(_expectedProduct);
      _existingProduct.ShortName = "ShortName0";
      _upgradeProduct.ShortName = "ShortName0";
      _existingProduct.UpgardeData(_upgradeProduct);
      Assert.AreEqual<string>(_existingProduct.ShortName, _upgradeProduct.ShortName);
    }
    [TestMethod]
    [ExpectedException(typeof(LicenseFileException))]
    public void DifferentNamesExceptionTestMethod()
    {
      Assembly _expectedProduct = Assembly.GetExecutingAssembly();
      Product _existingProduct = new Product(_expectedProduct);
      Product _upgradeProduct = new Product(_expectedProduct);
      _existingProduct.ShortName = "ShortName0";
      _upgradeProduct.ShortName = "ShortName1";
      _existingProduct.UpgardeData(_upgradeProduct);
    }
  }
}
