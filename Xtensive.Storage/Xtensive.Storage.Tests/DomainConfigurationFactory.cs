// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.08.05
//

using System;
using Xtensive.Storage.Configuration;
using Xtensive.Storage.Model;

namespace Xtensive.Storage.Tests
{
  public static class DomainConfigurationFactory
  {
    private const string StorageTypeKey =          "X_STORAGE";
    private const string ForeignKeysModeKey =      "X_FOREIGN_KEYS";
    private const string TypeIdKey =               "X_TYPE_ID";
    private const string InheritanceSchemaKey =    "X_INHERITANCE_SCHEMA";

    public static DomainConfiguration Create()
    {
      // Default values
      var storageType = "memory";
      var foreignKeyMode = ForeignKeyMode.Default;
      var typeIdBehavior = TypeIdBehavior.Default;
      var inheritanceSchema = InheritanceSchema.Default;

      // Getting values from the environment variables
      var value = GetEnvironmentVariable(StorageTypeKey);
      if (!string.IsNullOrEmpty(value))
        storageType = value;

      value = GetEnvironmentVariable(TypeIdKey);
      if (!string.IsNullOrEmpty(value)) {
        typeIdBehavior = (TypeIdBehavior) Enum.Parse(typeof (TypeIdBehavior), value, true);
      }

      value = GetEnvironmentVariable(InheritanceSchemaKey);
      if (!string.IsNullOrEmpty(value)) {
        inheritanceSchema = (InheritanceSchema) Enum.Parse(typeof (InheritanceSchema), value, true);
      }

      value = GetEnvironmentVariable(ForeignKeysModeKey);
      if (!string.IsNullOrEmpty(value)) {
        foreignKeyMode = (ForeignKeyMode) Enum.Parse(typeof (ForeignKeyMode), value, true);
      }

      DomainConfiguration config;

      config = Create(storageType, inheritanceSchema, typeIdBehavior, foreignKeyMode);

      // Here you still have the ability to override the above values

//      config = Create("memory");
//      config = Create("memory", InheritanceSchema.SingleTable);
//      config = Create("memory", InheritanceSchema.ConcreteTable);
//      config = Create("memory", InheritanceSchema.Default, TypeIdBehavior.Include);

//      config = Create("mssql2005");
//      config = Create("mssql2005", InheritanceSchema.SingleTable);
//      config = Create("mssql2005", InheritanceSchema.ConcreteTable);
//      config = Create("mssql2005", InheritanceSchema.Default, TypeIdBehavior.Include);

//      config = Create("pgsql");
//      config = Create("pgsql", InheritanceSchema.SingleTable);
//      config = Create("pgsql", InheritanceSchema.ConcreteTable);
//      config = Create("pgsql", InheritanceSchema.Default, TypeIdBehavior.Include);
      return config;
    }

    public static DomainConfiguration Create(string protocol)
    {
      ConcreteTableSchemaModifier.IsEnabled = false;
      SingleTableSchemaModifier.IsEnabled = false;
      ClassTableSchemaModifier.IsEnabled = false;
      IncludeTypeIdModifier.IsEnabled = false;
      ExcludeTypeIdModifier.IsEnabled = false;
      TypeIdModifier.IsEnabled = false;
      return DomainConfiguration.Load(protocol);
    }

    public static DomainConfiguration Create(string protocol, InheritanceSchema schema)
    {
      ConcreteTableSchemaModifier.IsEnabled = false;
      SingleTableSchemaModifier.IsEnabled = false;
      ClassTableSchemaModifier.IsEnabled = false;
      IncludeTypeIdModifier.IsEnabled = false;
      ExcludeTypeIdModifier.IsEnabled = false;
      TypeIdModifier.IsEnabled = false;
      DomainConfiguration config = Create(protocol);
      if (schema != InheritanceSchema.Default)
        InheritanceSchemaModifier.ActivateModifier(schema);
      return config;
    }

    public static DomainConfiguration Create(string protocol, InheritanceSchema schema, TypeIdBehavior typeIdBehavior)
    {
      IncludeTypeIdModifier.IsEnabled = false;
      ExcludeTypeIdModifier.IsEnabled = false;
      TypeIdModifier.IsEnabled = false;
      DomainConfiguration config = Create(protocol, schema);
      if (typeIdBehavior != TypeIdBehavior.Default)
        TypeIdModifier.ActivateModifier(typeIdBehavior);
      return config;
    }

    public static DomainConfiguration Create(string protocol, InheritanceSchema schema, TypeIdBehavior typeIdBehavior, ForeignKeyMode foreignKeyMode)
    {
      IncludeTypeIdModifier.IsEnabled = false;
      ExcludeTypeIdModifier.IsEnabled = false;
      TypeIdModifier.IsEnabled = false;
      DomainConfiguration config = Create(protocol, schema);
      if (typeIdBehavior != TypeIdBehavior.Default)
        TypeIdModifier.ActivateModifier(typeIdBehavior);
      config.ForeignKeyMode = foreignKeyMode;
      return config;
    }

    private static string GetEnvironmentVariable(string key)
    {
      string result = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
      if (!string.IsNullOrEmpty(result))
        return result;
      return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
    }
  }
}
