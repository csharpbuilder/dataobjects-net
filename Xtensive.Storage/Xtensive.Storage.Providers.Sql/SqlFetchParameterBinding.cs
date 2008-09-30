// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.09.25

using System;
using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Storage.Providers.Sql.Mappings;

namespace Xtensive.Storage.Providers.Sql
{
  public sealed class SqlFetchParameterBinding : SqlParameterBinding<Func<object>>
  {


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="typeMapping">The type mapping.</param>
    /// <param name="valueAccessor">The value accessor.</param>
    public SqlFetchParameterBinding(Func<object> valueAccessor, DataTypeMapping typeMapping)
      : base(valueAccessor, typeMapping)
    {
    }

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="valueAccessor">The value accessor.</param>
    public SqlFetchParameterBinding(Func<object> valueAccessor)
      : base(valueAccessor, null)
    {
    }
  }
}