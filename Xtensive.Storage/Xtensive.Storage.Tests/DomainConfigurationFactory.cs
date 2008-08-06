// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.08.05

using Xtensive.Core;
using Xtensive.Storage.Configuration;

namespace Xtensive.Storage.Tests
{
  public static class DomainConfigurationFactory
  {
    public static DomainConfiguration Create(string protocol)
    {
      DomainConfiguration config = new DomainConfiguration();
      config.ConnectionInfo = new UrlInfo(protocol+@"://localhost/Test_4.0");
      return config;
    }

    public static DomainConfiguration Create(string protocol, InheritanceSchema schema)
    {
      DomainConfiguration config = Create(protocol);
      config.Builders.Add(InheritanceSchemaModifier.GetModifier(schema));
      return config;
    }

    public static DomainConfiguration Create(string protocol, InheritanceSchema schema, TypeIdBehavior typeIdBehavior)
    {
      DomainConfiguration config = Create(protocol, schema);
      config.Builders.Add(TypeIdModifier.GetModifier(typeIdBehavior));
      return config;
    }
  }
}