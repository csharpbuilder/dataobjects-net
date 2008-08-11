// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.08.11

using Xtensive.Storage.Rse.Providers;
using Xtensive.Storage.Rse.Providers.Compilable;

namespace Xtensive.Storage.Rse.Compilation
{
  internal sealed class TakeProviderCompiler : TypeCompiler<TakeProvider>
  {
    protected override ExecutableProvider Compile(TakeProvider provider)
    {
      return new Providers.Executable.TakeProvider(
        provider,
        Compiler.Compile(provider.Source, true),
        provider.Count);
    }

    // Constructors

    public TakeProviderCompiler(Compiler compiler)
      : base(compiler)
    {
    }
  }
}