// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Kofman
// Created:    2009.05.27

using System;
using System.Collections.Generic;
using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Integrity.Resources;

namespace Xtensive.Integrity.Aspects.Constraints
{
  /// <summary>
  /// Ensures that date value is in the future.
  /// </summary>
  [Serializable]
  public class FutureConstraint : PropertyConstraintAspect
  {
    /// <inheritdoc/>
    public override bool IsSupported(Type valueType)
    {
      return valueType == typeof (DateTime);
    }

    /// <inheritdoc/>
    public override bool IsValid(object value)
    {
      return (DateTime) value > DateTime.Now;
    }

    protected override string GetDefaultMessage()
    {
      return Strings.ConstraintMessageValueMustBeInTheFuture;
    }
  }
}