// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Kofman
// Created:    2009.04.03

using System;
using Xtensive.Core.Internals.DocTemplates;

namespace Xtensive.Core.Tuples
{
  /// <summary>
  /// Tuple that references another tuple by getter delegate.
  /// </summary>
  public class ReferencedTuple : Tuple
  {
    private readonly Func<Tuple> tupleGetter;

    private Tuple InnerTuple
    {
      get { return tupleGetter.Invoke(); }
    }

    /// <inheritdoc/>
    public override TupleFieldState GetFieldState(int fieldIndex)
    {
      return InnerTuple.GetFieldState(fieldIndex);
    }

    /// <inheritdoc/>
    public override object GetValue(int fieldIndex, out TupleFieldState fieldState)
    {
      return InnerTuple.GetValue(fieldIndex, out fieldState);
    }

    /// <inheritdoc/>
    public override void SetValue(int fieldIndex, object fieldValue)
    {
      InnerTuple.SetValue(fieldIndex, fieldValue);
    }

    /// <inheritdoc/>
    public override TupleDescriptor Descriptor
    {
      get { return InnerTuple.Descriptor; }
    }

    public override Pair<Tuple, int> GetMappedContainer(int fieldIndex, bool isWriting)
    {
      return InnerTuple.GetMappedContainer(fieldIndex, isWriting);
    }


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="tupleGetter">The delegate to get inner tuple.</param>
    public ReferencedTuple(Func<Tuple> tupleGetter)
    {
      this.tupleGetter = tupleGetter;
    }
  }
}