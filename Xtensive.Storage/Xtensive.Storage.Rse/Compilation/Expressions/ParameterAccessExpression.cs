// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.11.11

using System;
using System.Linq.Expressions;

namespace Xtensive.Storage.Rse.Compilation.Expressions
{
  public class ParameterAccessExpression : Expression
  {
    public Expression<Func<object>> Binding { get; private set; }

    public override string ToString()
    {
      return "Param : " + Binding;
    }

    // Constructors

    public ParameterAccessExpression(Type type, Expression<Func<object>> binding)
      : base((ExpressionType)ExtendedExpressionType.ParameterAccess, type)
    {
      Binding = binding;
    }
  }
}