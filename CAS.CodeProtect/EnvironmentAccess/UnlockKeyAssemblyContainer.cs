//<summary>
//  Title   : Unlock Key Assembly Container
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2011, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@UAOOI.eu
//  http://www.UAOOI.eu
//</summary>

using UAOOI.CodeProtect.LicenseDsc;
using UAOOI.CodeProtect.Properties;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace UAOOI.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Class UnlockKeyAssemblyContainer - Unlock Key Assembly Container
  /// </summary>
  public class UnlockKeyAssemblyContainer
  {
    #region private
    private const ushort minCodeLength = 5;
    private const string CodeSurroundString = "__";// code in filename has to be surrounded by two underscores
    private Assembly m_Container;
    private StringBuilder LastErrorMessage = new StringBuilder();
    private Assembly LoadContainer(string assemblyPath)
    {
      try
      {
        return Assembly.Load(assemblyPath);
      }
      catch (Exception ex)
      {
        LastErrorMessage.AppendLine(string.Format(Resources.ErrStr_InstallLicense_CannotLoadContainer, ex.Message));
        return null;
      }
    }
    #endregion

    #region API
    /// <summary>
    /// Gets the manifest resource stream.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>Stream.</returns>
    /// <exception cref="UAOOI.CodeProtect.LicenseDsc.LicenseFileException">
    /// </exception>
    public Stream GetManifestResourceStream(string path)
    {
      if (LastErrorMessage.Length > 0)
        throw new LicenseFileException(LastErrorMessage.ToString());
      if (m_Container == null)
        throw new LicenseFileException(Resources.ErrStr_InstallLicense_NoContainer);
      if (string.IsNullOrEmpty(path))
        throw new LicenseFileException(Resources.ErrStr_InstallLicense_NoPath);
      try
      {
        return m_Container.GetManifestResourceStream(path);
      }
      catch (Exception ex)
      {
        throw new LicenseFileException(Resources.ErrStr_InstallLicense_NoLicenseFound, ex);
      }
    }
    /// <summary>
    /// Gets the manifest resource path.
    /// </summary>
    /// <param name="EnteredCode">The entered code.</param>
    /// <returns>System.String.</returns>
    public string GetManifestResourcePath(string EnteredCode)
    {
      if (m_Container == null)
        LastErrorMessage.AppendLine(Resources.ErrStr_InstallLicense_NoContainer);
      string path = string.Empty;
      string code = EnteredCode.Trim();
      string productName = string.Empty;
      if (FileNames.ProductManifest() != null)
        productName = FileNames.ProductManifest().Product;
      else
      {
        LastErrorMessage.AppendLine(Resources.ErrStr_InstallLicense_NoManifest);
        return string.Empty;
      }
      bool productFoundInContainer = false;
      productName = productName.Replace(Resources.ProductUnlockCodeStringToRemove, "");
      string productNameWithDots = String.Format(".{0}.", productName);
      if (!String.IsNullOrEmpty(code) && code.Length >= minCodeLength && m_Container != null)
      {
        string codeWithSurrounding = string.Format("{0}{1}{0}", CodeSurroundString, code);
        foreach (string item in m_Container.GetManifestResourceNames())
        {
          if (item.Contains(productNameWithDots))
          {
            productFoundInContainer = true;
            if (item.Contains(codeWithSurrounding))
            {
              path = item;
              break;
            }
          }
        }
      }
      if (!productFoundInContainer)
        LastErrorMessage.AppendLine(string.Format(Resources.ErrStr_InstallLicense_NoLicForProductInContainer, productName));
      return path;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockKeyAssemblyContainer"/> class.
    /// </summary>
    public UnlockKeyAssemblyContainer()
      : this(Resources.CASCommonResources)
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockKeyAssemblyContainer"/> class.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    public UnlockKeyAssemblyContainer(Assembly assembly)
    {
      m_Container = assembly;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockKeyAssemblyContainer"/> class.
    /// </summary>
    /// <param name="assemblyName">Name of the assembly.</param>
    public UnlockKeyAssemblyContainer(string assemblyName)
    {
      m_Container = LoadContainer(assemblyName);
    }
    #endregion

  }
}