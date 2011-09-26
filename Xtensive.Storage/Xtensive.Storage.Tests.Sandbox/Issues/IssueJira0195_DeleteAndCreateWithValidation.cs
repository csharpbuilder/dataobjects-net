﻿// Copyright (C) 2011 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2011.09.23

using System;
using NUnit.Framework;
using Xtensive.Integrity.Validation;
using Xtensive.Storage.Configuration;
using Xtensive.Storage.Tests.Issues.IssueJira0195_DeleteAndCreateWithValidationModel;

namespace Xtensive.Storage.Tests.Issues.IssueJira0195_DeleteAndCreateWithValidationModel
{
  [HierarchyRoot]
  public class Guider : Entity
  {
    [Field, Key]
    public Guid Id { get; private set; }

    [Field(Nullable = false)]
    public string Name { get; set; }

    protected override void OnValidate()
    {
      if (IsRemoved)
        throw new InvalidOperationException("Should not validate removed entity");
    }

    public Guider(Guid id)
      : base(id)
    {
    }
  }
}

namespace Xtensive.Storage.Tests.Issues
{
  public class IssueJira0195_DeleteAndCreateWithValidation : AutoBuildTest
  {
    protected override DomainConfiguration BuildConfiguration()
    {
      var config = base.BuildConfiguration();
      config.AutoValidation = true;
      config.ValidationMode = ValidationMode.OnDemand;
      config.Types.Register(typeof (Guider).Assembly, typeof (Guider).Namespace);
      return config;
    }

    [Test]
    public void MainTest()
    {
      using (Session.Open(Domain)) {
        using (var t = Transaction.Open()) {
          var id = Guid.NewGuid();
          new Guider(id) {Name = "123"};
          var g = Query.Single<Guider>(id);
          g.Remove();
          new Guider(id) {Name = "321"};
          Assert.IsTrue(g.IsRemoved); // Check that IsRemoved is accessible
          g.Validate(); // Check that Validate() is no-op for removed entities
          t.Complete();
        }
      }
    }
  }
}