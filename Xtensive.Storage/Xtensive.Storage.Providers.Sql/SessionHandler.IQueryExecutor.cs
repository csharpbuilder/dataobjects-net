// Copyright (C) 2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Yakunin
// Created:    2010.02.09

using System.Collections.Generic;
using Xtensive.Core.Tuples;
using Tuple = Xtensive.Core.Tuples.Tuple;
using Xtensive.Sql;

namespace Xtensive.Storage.Providers.Sql
{
  public partial class SessionHandler
  {
    // Implementation of IQueryExecutor

    /// <inheritdoc/>
    IEnumerator<Tuple> IQueryExecutor.ExecuteTupleReader(QueryRequest request)
    {
      lock (connectionSyncRoot) {
        EnsureConnectionIsOpen();
        var enumerator = commandProcessor.ExecuteRequestsWithReader(request);
        using (enumerator) {
          while (enumerator.MoveNext())
            yield return enumerator.Current;
        }
      }
    }

    /// <inheritdoc/>
    int IQueryExecutor.ExecuteNonQuery(ISqlCompileUnit statement)
    {
      lock (connectionSyncRoot) {
        EnsureConnectionIsOpen();
        using (var command = connection.CreateCommand(statement))
          return driver.ExecuteNonQuery(Session, command);
      }
    }

    /// <inheritdoc/>
    object IQueryExecutor.ExecuteScalar(ISqlCompileUnit statement)
    {
      lock (connectionSyncRoot) {
        EnsureConnectionIsOpen();
        using (var command = connection.CreateCommand(statement))
          return driver.ExecuteScalar(Session, command);
      }
    }

    /// <inheritdoc/>
    int IQueryExecutor.ExecuteNonQuery(string commandText)
    {
      lock (connectionSyncRoot) {
        EnsureConnectionIsOpen();
        using (var command = connection.CreateCommand(commandText))
          return driver.ExecuteNonQuery(Session, command);
      }
    }

    /// <inheritdoc/>
    object IQueryExecutor.ExecuteScalar(string commandText)
    {
      lock (connectionSyncRoot) {
        EnsureConnectionIsOpen();
        using (var command = connection.CreateCommand(commandText))
          return driver.ExecuteScalar(Session, command);
      }
    }

    /// <inheritdoc/>
    void IQueryExecutor.Store(TemporaryTableDescriptor descriptor, IEnumerable<Tuple> tuples)
    {
      lock (connectionSyncRoot) {
        EnsureConnectionIsOpen();
        foreach (var tuple in tuples)
          commandProcessor.RegisterTask(new SqlPersistTask(descriptor.StoreRequest, tuple));
        commandProcessor.ExecuteRequests();
      }
    }

    /// <inheritdoc/>
    void IQueryExecutor.Clear(TemporaryTableDescriptor descriptor)
    {
      lock (connectionSyncRoot) {
        EnsureConnectionIsOpen();
        commandProcessor.RegisterTask(new SqlPersistTask(descriptor.ClearRequest, null));
        commandProcessor.ExecuteRequests();
      }
    }
  }
}