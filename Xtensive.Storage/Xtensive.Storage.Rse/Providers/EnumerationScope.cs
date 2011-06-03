// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.07.21

using System;
using Xtensive.Core;
using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Core.IoC;
using Xtensive.Storage.Rse.Compilation;
using Xtensive.Storage.Rse.Resources;

namespace Xtensive.Storage.Rse.Providers
{
  /// <summary>
  /// <see cref="EnumerationContext"/> activation scope.
  /// </summary>
  public class EnumerationScope : InheritableScope<EnumerationContext, EnumerationScope>
  {
    /// <summary>
    /// Gets the current context.
    /// </summary>
    public new static EnumerationContext CurrentContext
    {
      get { return Scope<EnumerationContext>.CurrentContext; }
    }

    /// <summary>
    /// Gets the context of this scope.
    /// </summary>
    public new EnumerationContext Context
    {
      get { return base.Context; }
    }

    /// <summary>
    /// Create the new <see cref="EnumerationScope"/> using 
    /// <see cref="CompilationContext.CreateEnumerationContext"/> method of the
    /// <see cref="CompilationContext.Current"/> compilation context, 
    /// if <see cref="CurrentContext"/> is <see langword="null" />.
    /// Otherwise, returns <see langword="null" />.
    /// </summary>
    /// <returns>Either new <see cref="EnumerationScope"/> or <see langword="null" />.</returns>
    /// <exception cref="InvalidOperationException">Active <see cref="CompilationContext"/> absents.</exception>
    public static EnumerationScope Open()
    {
      if (CurrentContext!=null)
        return null;
      var compilationContext = CompilationContext.Current;
      if (compilationContext==null)
        throw new InvalidOperationException(
          Strings.ExCantOpenEnumerationScopeSinceThereIsNoCurrentCompilationContext);        
      return compilationContext.CreateEnumerationContext().Activate();
    }

    /// <summary>
    /// Create the new <see cref="EnumerationScope"/> having
    /// <see cref="EnumerationContext"/> property set to <see langword="null" />, 
    /// if <see cref="CurrentContext"/> is not <see langword="null" />.
    /// Otherwise, returns <see langword="null" />.
    /// In fact, temporarily blocks current <see cref="EnumerationContext"/>
    /// and ensures next call to <see cref="Open"/> will return 
    /// a new <see cref="EnumerationScope"/>.
    /// </summary>
    /// <returns>Either new <see cref="EnumerationScope"/> or <see langword="null" />.</returns>
    public static EnumerationScope Block()
    {
      return CurrentContext==null ? null : new EnumerationScope(null);
    }


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="context">The context.</param>
    public EnumerationScope(EnumerationContext context)
      : base(context)
    {
    }
  }
}