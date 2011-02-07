// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2010.03.03

using System;
using System.Linq;
using NUnit.Framework;
using Xtensive.Storage.Configuration;
using Xtensive.Storage.Tests.Storage.LegacyDb.CrazyColumns2005TestModel;

namespace Xtensive.Storage.Tests.Storage.LegacyDb.CrazyColumns2005TestModel
{
  [Serializable]
  [HierarchyRoot]
  public class Crazy : Entity
  {
    [Field, Key]
    public Guid Id { get; private set; }

    [Field]
    public DateTime SmallDateTime { get; set; }
  }
}

namespace Xtensive.Storage.Tests.Storage.LegacyDb
{
  [TestFixture]
  public class CrazyColumns2005Test : LegacyDbAutoBuildTest
  {
    protected override void CheckRequirements()
    {
      base.CheckRequirements();
      Require.ProviderVersionAtLeast(StorageProviderVersion.SqlServer2005);
    }

    protected override DomainConfiguration BuildConfiguration()
    {
      var config = base.BuildConfiguration();
      config.Types.Register(typeof(Crazy));
      return config;
    }

    [Test]
    public void CombinedTest()
    {
      var now = DateTime.Now;
      var theDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
      using (Session.Open(Domain)) {
        using (var ts = Transaction.Open()) {
          var crazy = new Crazy {SmallDateTime = theDate};
          ts.Complete();
        }
      }

      using (Session.Open(Domain))
      using (Transaction.Open()) {
        foreach (var item in Query.All<Crazy>())
          Console.WriteLine(item.SmallDateTime);
        Assert.AreEqual(1, Query.All<Crazy>().Where(o => o.SmallDateTime==theDate).Count());
      }
    }

    protected override string GetCreateDbScript(DomainConfiguration config)
    {
      return @"
        CREATE TABLE [dbo].[Crazy](
          [Id] [uniqueidentifier] NOT NULL,
          [SmallDateTime] [smalldatetime] NOT NULL,
          CONSTRAINT [PK_Crazy] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
        ) ON [PRIMARY]";      
    }
  }
}