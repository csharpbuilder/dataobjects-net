// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2009.07.15

using System;
using Xtensive.Sql.Resources;

namespace Xtensive.Sql.Info
{
  /// <summary>
  /// Various extension methods related to this namespace.
  /// </summary>
  public static class FeaturesExtensions
  {
    /// <summary>
    /// Determines whether the specified active features is supported.
    /// </summary>
    public static bool Supports(this QueryFeatures available, QueryFeatures required)
    {
      return (available & required)==required;
    }

    /// <summary>
    /// Determines whether the specified active features is supported.
    /// </summary>
    public static bool Supports(this IndexFeatures available, IndexFeatures required)
    {
      return (available & required)==required;
    }

    /// <summary>
    /// Determines whether the specified active features is supported.
    /// </summary>
    public static bool Supports(this DataTypeFeatures available, DataTypeFeatures required)
    {
      return (available & required)==required;
    }

    /// <summary>
    /// Determines whether the specified active features is supported.
    /// </summary>
    public static bool Supports(this ForeignKeyConstraintFeatures available, ForeignKeyConstraintFeatures required)
    {
      return (available & required)==required;
    }

    /// <summary>
    /// Determines whether the specified active features is supported.
    /// </summary>
    public static bool Supports(this ColumnFeatures available, ColumnFeatures required)
    {
      return (available & required)==required;
    }

    /// <summary>
    /// Determines whether the specified active features is supported.
    /// </summary>
    public static bool Supports(this ServerFeatures available, ServerFeatures required)
    {
      return (available & required)==required;
    }
  }
}