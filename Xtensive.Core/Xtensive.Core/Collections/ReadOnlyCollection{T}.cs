// Copyright (C) 2007 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Ilyin
// Created:    2007.06.04

using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Core.Resources;
using Xtensive.Core.Threading;

namespace Xtensive.Core.Collections
{
  /// <summary>
  /// Read-only collection (<see cref="ICollection{T}"/>) wrapper.
  /// </summary>
  /// <typeparam name="T">The type of collection items.</typeparam>
  [Serializable]
  public class ReadOnlyCollection<T> : 
    ICollection,
    ICollection<T>,
    ICountable<T>,
    ISynchronizable,
    IReadOnly
  {
    private readonly ICollection<T> innerCollection;

    /// <inheritdoc/>
    [DebuggerHidden]
    public int Count
    {
      get { return innerCollection.Count; }
    }

    /// <inheritdoc/>
    [DebuggerHidden]
    long ICountable.Count
    {
      get { return Count; }
    }

    /// <inheritdoc/>
    [DebuggerHidden]
    public object SyncRoot
    {
      get { return this; }
    }

    #region IsXxx properties

    /// <inheritdoc/>
    [DebuggerHidden]
    public virtual bool IsSynchronized
    {
      get { return false; }
    }

    /// <summary>
    /// Always returns <see langword="true"/>.
    /// </summary>
    /// <returns><see langword="True"/>.</returns>
    [DebuggerHidden]
    bool ICollection<T>.IsReadOnly
    {
      get { return true; }
    }

    #endregion

    #region Contains, CopyTo methods

    /// <summary>
    /// Indicates whether this collection is a read-only wrapper 
    /// of specified <paramref name="collection"/>.
    /// </summary>
    /// <param name="collection">Collection to check.</param>
    /// <returns><see langword="True"/> if this collection is wrapper of
    /// specified collection; otherwise, <see langword="false"/>.</returns>
    public bool IsWrapperOf(ICollection<T> collection)
    {
      return innerCollection==collection;
    }
    
    /// <inheritdoc/>
    public bool Contains(T item)
    {
      return innerCollection.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(Array array, int index)
    {
      if (innerCollection is ICollection)
        ((ICollection)innerCollection).CopyTo(array, index);
      else
        innerCollection.Copy(array, index);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
      innerCollection.CopyTo(array, arrayIndex);
    }

    #endregion

    #region Exceptions on: Add, Remove, Clear methods

    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">Always thrown by this method.</exception>
    public void Add(T item)
    {
      throw Exceptions.CollectionIsReadOnly(null);
    }

    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">Always thrown by this method.</exception>
    public bool Remove(T item)
    {
      throw Exceptions.CollectionIsReadOnly(null);
    }

    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">Always thrown by this method.</exception>
    public void Clear()
    {
      throw Exceptions.CollectionIsReadOnly(null);
    }

    #endregion

    #region GetEnumerator methods

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
      return innerCollection.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return innerCollection.GetEnumerator();
    }

    #endregion


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true" />
    /// </summary>
    /// <param name="collection">The collection to copy or wrap.</param>
    /// <param name="copy">Indicates whether <paramref name="collection"/> must be copied or wrapped.</param> 
    public ReadOnlyCollection(ICollection<T> collection, bool copy)
    {
      ArgumentValidator.EnsureArgumentNotNull(collection, "collection");
      if (!copy)
        innerCollection = collection;
      else
        innerCollection = new List<T>(collection);
    }

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true" />
    /// </summary>
    /// <param name="collection">The collection to wrap.</param>
    public ReadOnlyCollection(ICollection<T> collection) 
      : this(collection, false)
    {
    }
  }
}