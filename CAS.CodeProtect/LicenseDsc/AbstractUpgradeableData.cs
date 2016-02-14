//<summary>
//  Title   : Instantiated objects derived from this classes represent upgradeable data.
//  System  : Microsoft Visual C# .NET 2008
//  $LastChangedDate$
//  $Rev$
//  $LastChangedBy$
//  $URL$
//  $Id$
//
//  Copyright (C)2008, CAS LODZ POLAND.
//  TEL: +48 (42) 686 25 47
//  mailto://techsupp@cas.eu
//  http://www.cas.eu
//</summary>

using System;

namespace CAS.Lib.CodeProtect.LicenseDsc
{
  /// <summary>
  /// Instantiated objects derived from this classes represent upgradeable data.
  /// </summary>
  /// <typeparam name="Type">Derived type</typeparam>
  public abstract class AbstractUpgradeableData<Type>: AbstractLicenseData
  {
    /// <summary>
    /// Modifies an old value if the new one is not empty.
    /// </summary>
    /// <param name="oldVal">Value to be modified</param>
    /// <param name="newVal">New value</param>
    protected void UpgradeField( ref string oldVal, string newVal )
    {
      if ( newVal != string.Empty )
        oldVal = newVal;
    }
    /// <summary>
    /// Upgrade the this object
    /// </summary>
    /// <param name="upgrade">Object to be used to upgrade this object</param>
    /// <returns>false if upgrade is impossible due to inconsistency</returns>
    public abstract Boolean UpgardeData( Type upgrade );
  }
}
