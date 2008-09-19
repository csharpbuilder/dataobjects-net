// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Elena Vakhtina
// Created:    2008.09.18

using System;
using System.Collections.Generic;
using Xtensive.Core.Comparison;
using Xtensive.Core.Tuples;
using Xtensive.Core.Tuples.Transform;

namespace Xtensive.Storage.Rse.Providers.Executable
{
  internal class OrderedGroupProvider : UnaryExecutableProvider<Compilable.AggregateProvider>
  {
    protected internal override IEnumerable<Tuple> OnEnumerate(EnumerationContext context)
    {
      Tuple lastTuple = null;
      int groupIndex = -1;
      var calculator = new AggregateCalculatorProvider(Origin.AggregateColumns);
      var actionList = new List<Action<Tuple, Tuple, int>>();
      var result = new List<Tuple>();

      foreach (var col in Origin.AggregateColumns)
        actionList.Add((Action<Tuple, Tuple, int>)typeof(AggregateCalculatorProvider).GetMethod("GetAggregateCalculator")
            .MakeGenericMethod(col.Type).Invoke(calculator, new object[] { col.AggregateType, col.SourceIndex, col.Index}));

      foreach (var tuple in Source.Enumerate(context)){
        var resultTuple = Origin.Transform.Apply(TupleTransformType.Tuple, tuple);
        if (!AdvancedComparer<Tuple>.Default.Equals(lastTuple,resultTuple)){
          groupIndex++;
          result.Add(Tuple.Create(Origin.Header.TupleDescriptor));
          resultTuple.CopyTo(result[groupIndex]);
          lastTuple = resultTuple;
        }
        foreach (var col in Origin.AggregateColumns) {
          actionList[col.Index - Origin.GroupColumnIndexes.Length](tuple, calculator.GetAccumulator(col.Index, groupIndex), groupIndex);
        }
      }

      for (int i = 0; i <= groupIndex; i++) {
        foreach (var col in Origin.AggregateColumns)
          result[i] = (Tuple)typeof(AggregateCalculatorProvider).GetMethod("Calculate")
            .MakeGenericMethod(col.Type).Invoke(calculator, new object[] { col, calculator.GetAccumulator(col.Index, i), result[i] });
      }
      return result;
    }


    // Constructor

    public OrderedGroupProvider(Compilable.AggregateProvider origin, ExecutableProvider source)
      : base(origin, source)
    {
    }

  }
}