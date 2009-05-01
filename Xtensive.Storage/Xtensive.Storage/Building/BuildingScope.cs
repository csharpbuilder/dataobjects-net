// Copyright (C) 2007 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2007.09.28

using Xtensive.Core;
using Xtensive.Core.Internals.DocTemplates;

namespace Xtensive.Storage.Building
{
  /// <summary>
  /// The scope for <see cref="BuildingContext"/>.
  /// </summary>
  public class BuildingScope: Scope<BuildingContext>
  {
    /// <summary>
    /// Gets the context.
    /// </summary>
    public new static BuildingContext Context
    {
      get { return CurrentContext; }
    }


    // Constructors

    internal BuildingScope(BuildingContext buildingContext)
      : base(buildingContext)
    {
    }
  }
}