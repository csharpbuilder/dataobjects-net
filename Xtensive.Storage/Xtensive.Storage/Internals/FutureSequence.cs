// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexander Nikolaev
// Created:    2009.08.19

using System;
using System.Collections;
using System.Collections.Generic;
using Xtensive.Core.Parameters;
using Xtensive.Storage.Linq;

namespace Xtensive.Storage.Internals
{
  [Serializable]
  internal sealed class FutureSequence<T> : FutureBase<IEnumerable<T>>,
    IEnumerable<T>
  {
    public IEnumerator<T> GetEnumerator()
    {
      return Materialize().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }


    // Constructors

    public FutureSequence(TranslatedQuery<IEnumerable<T>> translatedQuery, ParameterContext parameterContext) 
      : base(translatedQuery, parameterContext)
    {}
  }
}