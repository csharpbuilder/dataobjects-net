// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Yakunin
// Created:    2009.04.17

using System;
using NUnit.Framework;
using Xtensive.Core.Serialization.Binary;
using Xtensive.Modelling.Actions;
using Xtensive.Modelling.Comparison.Hints;
using Xtensive.Modelling.Tests.IndexingModel;

namespace Xtensive.Modelling.Tests
{
  [TestFixture]
  public class IndexingModelTest
  {
    [Test]
    public void CombinedTest()
    {
      var storage = CreateSimpleStorageModel();
      storage.Dump();

      TestUpdate(storage, (s1, s2, hs) => {
        var t1 = (TableInfo) s1.Resolve("Tables/Types");
        t1.Remove();
        var o2 = (TableInfo) s2.Resolve("Tables/Objects");
        o2.Remove();
      });
    }

    public static StorageInfo CreateSimpleStorageModel()
    {
      var storage = new StorageInfo("Storage");
      
      // Types table
      var t = new TableInfo(storage, "Types");
      var tId = new ColumnInfo(t, "Id") {
        Type = new TypeInfo(typeof (int), false)
      };
      var tValue = new ColumnInfo(t, "Value") {
        Type = new TypeInfo(typeof (string), 1024)
      };
      var tData = new ColumnInfo(t, "Data") {
        Type = new TypeInfo(typeof (byte[]), 1024*1024)
      };

      var tiPk = new PrimaryIndexInfo(t, "PK_Types");
      new KeyColumnRef(tiPk, tId);
      tiPk.PopulateValueColumns();

      var tiValue = new SecondaryIndexInfo(t, "IX_Value");
      new KeyColumnRef(tiValue, tValue);
      tiValue.PopulatePrimaryKeyColumns();

      // Objects table
      var o = new TableInfo(storage, "Objects");
      var oId = new ColumnInfo(o, "Id") {
        Type = new TypeInfo(typeof (long), false)
      };
      var oTypeId = new ColumnInfo(o, "TypeId") {
        Type = new TypeInfo(typeof (int), false)
      };
      var oValue = new ColumnInfo(o, "Value") {
        Type = new TypeInfo(typeof (string), 1024)
      };

      var oiPk = new PrimaryIndexInfo(o, "PK_Objects");
      new KeyColumnRef(oiPk, oId);
      oiPk.PopulateValueColumns();

      var oiTypeId = new SecondaryIndexInfo(o, "IX_TypeId");
      new KeyColumnRef(oiTypeId, oTypeId);
      oiTypeId.PopulatePrimaryKeyColumns();

      var oiValue = new SecondaryIndexInfo(o, "IX_Value");
      new KeyColumnRef(oiValue, oValue);
      new IncludedColumnRef(oiValue, oTypeId);
      oiValue.PopulatePrimaryKeyColumns();

      var ofkTypeId = new ForeignKeyInfo(o, "FK_TypeId") {
        PrimaryKey = tiPk, 
        ForeignKey = oiTypeId
      };

      storage.Validate();
      return storage;
    }

    #region Private methods

    private static void TestUpdate(StorageInfo origin, Action<StorageInfo, StorageInfo, HintSet> update)
    {
      TestUpdate(origin, update, true);
      TestUpdate(origin, update, false);
    }

    private static void TestUpdate(StorageInfo origin, Action<StorageInfo, StorageInfo, HintSet> update, bool useHints)
    {
      var s1 = Clone(origin);
      var s2 = Clone(origin);
      var hints = new HintSet(s1, s2);
      update.Invoke(s1, s2, hints);
      Log.Info("Update test ({0} hints)", useHints ? "with" : "without");
      s1.Dump();
      s2.Dump();

      // Comparing different models
      Log.Info("Comparing models:");
      var c = new Comparison.Comparer<StorageInfo>(s1, s2);
      if (useHints)
        foreach (var hint in hints)
          c.Hints.Add(hint);
      var diff = c.Difference;
      Log.Info("\r\nDifference:\r\n{0}", diff);
      var actions = new ActionSequence() { diff.ToActions() };
      Log.Info("\r\nActions:\r\n{0}", actions);
      actions.Apply(s1);
      s1.Dump();
      s2.Dump();

      // Comparing action applicaiton result & target model
      Log.Info("Comparing synchronization result:");
      c = new Comparison.Comparer<StorageInfo>(s1, s2);
      diff = c.Difference;
      Log.Info("\r\nDifference:\r\n{0}", diff);
      Assert.IsNull(diff);
    }

    private static StorageInfo Clone(StorageInfo server)
    {
      return (StorageInfo) LegacyBinarySerializer.Instance.Clone(server);
    }

    #endregion
  }
}