using System;
using System.Collections.Generic;
using System.Text;

namespace UAOOI.CodeProtect.LicenseDsc
{
  /// <summary>
  /// Iterface that allows accessing the items inside constraint
  /// </summary>
  public interface IConstraintItemProvider
  {
    /// <summary>
    /// Gets the constraints (items) list.
    /// </summary>
    /// <value>The items.</value>
    List<IConstraint> Items { get; }
  }
}
