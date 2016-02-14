//<summary>
//  Title   : Manifest Creator
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2009, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace CAS.Lib.CodeProtect.EnvironmentAccess
{
  /// <summary>
  /// Creates manifest for the product.
  /// </summary>
  internal static class ManifestManagement
  {
    #region internal
    /// <summary>
    /// Product type
    /// </summary>
    internal enum ProductType { AllUsers, SingleUser, ClickOnce }
    /// <summary>
    /// Writes the deploy manifest using data provided by the <paramref name="clickOnceAssembly"/>
    /// and the attributes the assembly is decorated.
    /// </summary>
    /// <param name="clickOnceAssembly">The click once assembly.</param>
    /// <param name="alternativeProductName">Name of the alternative product.
    /// If alternativeProductName is null or string.empty, the assembly name is taken.</param>
    /// <remarks>The type of the deployment is always <see cref="ProductType.ClickOnce"/></remarks>
    internal static void WriteDeployManifest( Assembly clickOnceAssembly, string alternativeProductName )
    {
      AssemblyName name= clickOnceAssembly.GetName();
      if ( !string.IsNullOrEmpty( alternativeProductName ) )
      {
        name.Name = alternativeProductName;
      }
      Uri url = new Uri( "http://www.commsvr.eu" );
      string publisher = GetPublisherFromAssemblyAttributes( clickOnceAssembly );
      WriteDeployManifest( name, ProductType.ClickOnce, publisher, url, url, FileNames.TargetDir );
    }
    /// <summary>
    /// Writes the deploy manifest.
    /// </summary>
    /// <param name="name">The name of the deployment.</param>
    /// <param name="type">The type <see cref="ProductType"/> of the deployment.</param>
    /// <param name="publisher">The publisher name - used to create the application data folder.</param>
    /// <param name="supportUrl">The support URL.</param>
    /// <param name="deploymentUrl">The deployment URL.</param>
    /// <param name="appData">The path for the application data folder.</param>
    internal static void WriteDeployManifest
      ( AssemblyName name, ProductType type, string publisher, Uri supportUrl, Uri deploymentUrl, string appData )
    {
      if ( File.Exists( FileNames.ManifestFileName ) )
        File.SetAttributes( FileNames.ManifestFileName, 0 );
      string hex = BitConverter.ToString( name.GetPublicKeyToken() );
      hex = hex.Replace( "-", "" );
      DeployManifest dm = new DeployManifest()
      {
        AssemblyIdentity = new AssemblyIdentity( name.FullName, name.Version.ToString() )
        {
          PublicKeyToken = hex,
          Type = type.ToString(),
        },
        Product = name.Name,
        Publisher = publisher,
        SupportUrl = supportUrl.ToString(),
        DeploymentUrl = deploymentUrl.ToString(),
      };
      FileReference fr = new FileReference()
      {
        TargetPath = appData,
        IsDataFile = true,
        IsOptional = false,
        Group = "license"
      };
      dm.FileReferences.Add( fr );
      ManifestWriter.WriteManifest( dm, FileNames.ManifestFileName );
      File.SetAttributes( FileNames.ManifestFileName, FileAttributes.ReadOnly | FileAttributes.Hidden );
    }
    /// <summary>
    /// Reads the deploy manifest using default file name
    /// </summary>
    /// <returns>
    /// Object of <see cref="DeployManifest"/> containing the product manifest
    /// </returns>
    internal static DeployManifest ReadDeployManifest()
    {
      return ReadDeployManifest( FileNames.ManifestFileName );
    }
    /// <summary>
    /// Reads the deploy manifest.
    /// </summary>
    /// <param name="ManifestFileName">Name of the manifest file.</param>
    /// <returns></returns>
    internal static DeployManifest ReadDeployManifest( string ManifestFileName )
    {
      if ( ( new FileInfo( ManifestFileName ) ).Exists )
      {
        Manifest mm = ManifestReader.ReadManifest( ManifestFileName, false );
        return mm as DeployManifest;
      }
      else
        return null;
    }
    /// <summary>
    /// Deletes the deploy manifest if exists.
    /// </summary>
    internal static void DeleteDeployManifest()
    {
      try
      {
        if ( File.Exists( FileNames.ManifestFileName ) )
        {
          File.SetAttributes( FileNames.ManifestFileName, 0 );
          File.Delete( FileNames.ManifestFileName );
        }
        FileNames.UnloadProductManifest();
      }
      catch { }
    }
    #endregion

    #region private
    private static string GetPublisherFromAssemblyAttributes( Assembly enteringAssembly )
    {
      string publisher = InstallContextNames.DefaultPublisherName;
      object[] attributes = enteringAssembly.GetCustomAttributes( false );
      foreach ( var item in attributes )
        if ( item is AssemblyCompanyAttribute )
          publisher = ( (AssemblyCompanyAttribute)item ).Company;
      return publisher;
    }
    #endregion
  }
}
