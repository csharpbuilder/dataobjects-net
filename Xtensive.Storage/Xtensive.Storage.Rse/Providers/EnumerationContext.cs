// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Yakunin
// Created:    2008.07.16

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xtensive.Core;
using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Storage.Rse.Resources;

namespace Xtensive.Storage.Rse.Providers
{
  /// <summary>
  /// The single enumeration attempt context for the <see cref="ExecutableProvider"/>.
  /// </summary>
  [Serializable]
  public sealed class EnumerationContext: Context<EnumerationScope>
  {
    private readonly Dictionary<Pair<object, object>, object> cache = new Dictionary<Pair<object, object>, object>();

    /// <summary>
    /// Caches the value in the current <see cref="EnumerationContext"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    public void SetValue<T>(object key, T value)
      where T: class
    {
      SetValue(new Pair<object, object>(null, key), value);
    }

    /// <summary>
    /// Gets the cached value from the current <see cref="EnumerationContext"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>Cached value with the specified key;
    /// <see langword="null"/>, if no cached value is found, or it is already expired.</returns>
    public T GetValue<T>(object key)
      where T: class
    {
      return GetValue<T>(new Pair<object, object>(null, key));
    }

    internal void SetValue<T>(Pair<object, object> key, T value)
      where T: class
    {
      cache[key] = value;
    }

    internal T GetValue<T>(Pair<object, object> key)
      where T: class
    {
      object result;
      if (cache.TryGetValue(key, out result))
        return result as T;
      else
        return null;
    }
    
    protected override EnumerationScope CreateActiveScope()
    {
      return new EnumerationScope(this);
    }

    /// <inheritdoc/>
    public override bool IsActive
    {
      get { return EnumerationScope.CurrentContext==this; }
    }


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    internal EnumerationContext()
    {
//      Provider = provider;
//      Provider.OnBeforeEnumerate(this);
    }

    // IDisposable methods

    /// <see cref="ClassDocTemplate.Dispose" copy="true"/>
    public void Dispose()
    {
//      Provider.OnAfterEnumerate(this);
    }
  }
}