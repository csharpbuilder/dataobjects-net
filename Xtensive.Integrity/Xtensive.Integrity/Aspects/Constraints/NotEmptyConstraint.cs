// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Kofman
// Created:    2009.05.27

using System;
using Xtensive.Integrity.Resources;

namespace Xtensive.Integrity.Aspects.Constraints
{
  /// <summary>
  /// Ensures thet property value is not <see cref="string.Empty"/>.
  /// </summary>
  [Serializable]
  public class NotEmptyConstraint : PropertyConstraintAspect
  {
    /// <inheritdoc/>
    public override bool IsSupported(Type valueType)
    {
      return valueType==typeof (string);
    }

    /// <inheritdoc/>
    public override bool IsValid(object value)
    {
      return (string) value!=string.Empty;
    }

    protected override string GetDefaultMessage()
    {
      return Strings.ConstraintMessageValueCanNotBeEmpty;
    }
  }
}