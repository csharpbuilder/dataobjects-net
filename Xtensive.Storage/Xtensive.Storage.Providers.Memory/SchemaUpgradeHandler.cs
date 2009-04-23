// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Ivan Galkin
// Created:    2009.04.09

using System;
using Xtensive.Storage.Building;
using Xtensive.Storage.Model.Conversion;
using Xtensive.Storage.Providers.Index;

namespace Xtensive.Storage.Providers.Memory
{
  [Serializable]
  public class SchemaUpgradeHandler : Index.SchemaUpgradeHandler
  {
    /// <summary>
    /// Gets the storage view.
    /// </summary>
    protected IndexStorageView StorageView
    {
      get
      {
        var session = BuildingContext.Current.SystemSessionHandler;
        var view = ((SessionHandler) session).StorageView;

        return view as IndexStorageView;
      }
    }

    /// <inheritdoc/>
    public override void ClearStorageSchema()
    {
      StorageView.ClearSchema();
    }

    /// <inheritdoc/>
    public override void UpdateStorageSchema()
    {
      var converter = new DomainModelConverter(Handlers.NameBuilder.BuildForeignKeyName,
        Handlers.NameBuilder.BuildForeignKeyName);
      var newSchema = converter.Convert(BuildingContext.Current.Model, StorageView.Storage.Name);
      StorageView.CreateNewSchema(newSchema);
    }
  }
}