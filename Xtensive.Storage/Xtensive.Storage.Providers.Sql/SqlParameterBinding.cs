// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.09.26

using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Sql.Dom;
using Xtensive.Storage.Providers.Sql.Mappings;

namespace Xtensive.Storage.Providers.Sql
{
  public abstract class SqlParameterBinding
  {
    /// <summary>
    /// Gets the type mapping.
    /// </summary>
    public DataTypeMapping TypeMapping { get; private set; }

    /// <summary>
    /// Gets the SQL parameter.
    /// </summary>
    public SqlParameter SqlParameter { get; private set; }


    // Constructor

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    protected SqlParameterBinding(DataTypeMapping typeMapping)
    {
      TypeMapping = typeMapping;
      SqlParameter = new SqlParameter();
    }
  }
}