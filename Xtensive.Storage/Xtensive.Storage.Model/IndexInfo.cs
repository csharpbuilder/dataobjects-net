// Copyright (C) 2007 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Ustinov
// Created:    2007.07.10

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xtensive.Core;
using Xtensive.Core.Collections;
using System.Linq;
using Xtensive.Core.Helpers;
using Xtensive.Core.Internals.DocTemplates;
using Xtensive.Core.Tuples;

namespace Xtensive.Storage.Model
{
  /// <summary>
  /// Describes a single index in terms of storage.
  /// </summary>
  [DebuggerDisplay("{Name}; Attributes = {Attributes}.")]
  [Serializable]
  public sealed class IndexInfo : MappingNode
  {
    private IndexAttributes attributes;
    private ColumnGroup columnGroup;
    private readonly DirectionCollection<ColumnInfo> keyColumns = new DirectionCollection<ColumnInfo>();
    private readonly ColumnInfoCollection valueColumns = new ColumnInfoCollection();
    private readonly ColumnInfoCollection includedColumns = new ColumnInfoCollection();
    //TODO: Don't create instances for physical indexes
    private readonly CollectionBaseSlim<IndexInfo> underlyingIndexes = new CollectionBaseSlim<IndexInfo>();
    private readonly TypeInfo declaringType;
    private readonly TypeInfo reflectedType;
    private readonly IndexInfo declaringIndex;
    private double fillFactor;
    private string shortName;
    private ReadOnlyList<ColumnInfo> columns;
    private TupleDescriptor tupleDescriptor;
    private TupleDescriptor keyTupleDescriptor;

    public string ShortName {
      [DebuggerStepThrough]
      get { return shortName; }
      [DebuggerStepThrough]
      set {
        this.EnsureNotLocked();
        shortName = value;
      }
    }

    public double FillFactor {
      [DebuggerStepThrough]
      get { return fillFactor; }
      [DebuggerStepThrough]
      set {
        this.EnsureNotLocked();
        fillFactor = value;
      }
    }

    public ColumnGroup Group {
      [DebuggerStepThrough]
      get { return columnGroup; }
      [DebuggerStepThrough]
      set {
        this.EnsureNotLocked();
        columnGroup = value;
      }
    }

    /// <summary>
    /// Gets a collection of all the columns that are included into the index.
    /// </summary>
    public ReadOnlyList<ColumnInfo> Columns {
      [DebuggerStepThrough]
      get {
        return columns;
      }
    }

    /// <summary>
    /// Gets a collection of columns that are included into the index as index key.
    /// </summary>
    public DirectionCollection<ColumnInfo> KeyColumns
    {
      [DebuggerStepThrough]
      get { return keyColumns; }
    }

    /// <summary>
    /// Gets a collection of non key columns that are included into the index as index value.
    /// </summary>
    public ColumnInfoCollection ValueColumns
    {
      [DebuggerStepThrough]
      get { return valueColumns; }
    }

    /// <summary>
    /// Gets a Collection of columns that are included into the index.
    /// </summary>
    public ColumnInfoCollection IncludedColumns
    {
      get { return includedColumns; }
    }

    /// <summary>
    /// Gets the tuple descriptor containing all the <see cref="Columns"/>.
    /// </summary>
    public TupleDescriptor TupleDescriptor
    {
      get { return tupleDescriptor; }
    }

    /// <summary>
    /// Gets the tuple descriptor containing just <see cref="KeyColumns"/>.
    /// </summary>
    public TupleDescriptor KeyTupleDescriptor
    {
      get { return keyTupleDescriptor; }
    }

    /// <summary>
    /// Gets the underlying indexes for this instance. 
    /// </summary>
    public CollectionBaseSlim<IndexInfo> UnderlyingIndexes
    {
      [DebuggerStepThrough]
      get { return underlyingIndexes; }
    }

    /// <summary>
    /// Gets the type that declares this member.
    /// </summary>
    public TypeInfo DeclaringType
    {
      [DebuggerStepThrough]
      get { return declaringType; }
    }

    /// <summary>
    /// Gets the type that was used to obtain this instance.
    /// </summary>
    public TypeInfo ReflectedType
    {
      [DebuggerStepThrough]
      get { return reflectedType; }
    }

    /// <summary>
    /// Gets the declaring index for this index.
    /// </summary>
    public IndexInfo DeclaringIndex
    {
      [DebuggerStepThrough]
      get { return declaringIndex; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is primary index.
    /// </summary>
    public bool IsPrimary
    {
      [DebuggerStepThrough]
      get { return (attributes & IndexAttributes.Primary) > 0; }
      [DebuggerStepThrough]
      set { 
        this.EnsureNotLocked();
        attributes = value
          ? (Attributes | IndexAttributes.Primary) & ~IndexAttributes.Secondary
          : (Attributes & ~IndexAttributes.Primary) | IndexAttributes.Secondary;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is unique index.
    /// </summary>
    public bool IsUnique
    {
      [DebuggerStepThrough]
      get { return (attributes & IndexAttributes.Unique) > 0; }
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    public IndexAttributes Attributes
    {
      [DebuggerStepThrough]
      get { return attributes; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is virtual index.
    /// </summary>
    public bool IsVirtual
    {
      [DebuggerStepThrough]
      get { return (Attributes & IndexAttributes.Virtual) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is secondary index.
    /// </summary>
    public bool IsSecondary
    {
      [DebuggerStepThrough]
      get { return (attributes & IndexAttributes.Secondary) > 0; }
    }

    /// <inheritdoc/>
    public override void Lock(bool recursive)
    {
      base.Lock(recursive);
      CreateColumns();
      if (!recursive)
        return;
      keyColumns.Lock(true);
      valueColumns.Lock(true);
      foreach (IndexInfo baseIndex in underlyingIndexes)
        baseIndex.Lock();
      underlyingIndexes.Lock();
      CreateTupleDescriptors();
    }

    private void CreateTupleDescriptors()
    {
      tupleDescriptor = TupleDescriptor.Create(
        from c in Columns select c.ValueType);
      keyTupleDescriptor = TupleDescriptor.Create(
        from c in KeyColumns select c.Key.ValueType);
    }

    private void CreateColumns()
    {
      var list = new List<ColumnInfo>(keyColumns.Select(pair => pair.Key));
      list.AddRange(valueColumns);
      columns = new ReadOnlyList<ColumnInfo>(list);
    }


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="declaringType">The <see cref="TypeInfo"/> that declares this instance.</param>
    /// <param name="indexAttributes"><see cref="IndexAttributes"/> attributes for this instance.</param>
    public IndexInfo(TypeInfo declaringType, IndexAttributes indexAttributes)
    {
      this.declaringType = declaringType;
      reflectedType = declaringType;
      declaringIndex = this;
      if (declaringType.IsInterface && (reflectedType.Attributes & TypeAttributes.Materialized) == 0) {
        if ((indexAttributes & IndexAttributes.Primary)==0 || declaringType.Hierarchy.Schema != InheritanceSchema.SingleTable)
          attributes |= IndexAttributes.Virtual | IndexAttributes.Union;
        else
          attributes |= IndexAttributes.Virtual | IndexAttributes.Filtered;
      }
      else
        attributes |= IndexAttributes.Real;
      attributes |= indexAttributes;
    }

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="reflectedType">Reflected type.</param>
    /// <param name="ancestorIndex">The ancestors index.</param>
    /// <param name="isAbstract">If set to <see langword="true"/> then 
    /// the flag <see cref="IndexAttributes.Abstract"/> will be added to 
    /// the property <see cref="Attributes"/>.</param>
    public  IndexInfo(TypeInfo reflectedType,  IndexInfo ancestorIndex, bool isAbstract)
    {
      declaringType = ancestorIndex.DeclaringType;
      this.reflectedType = reflectedType;
      if (reflectedType.IsInterface && (reflectedType.Attributes & TypeAttributes.Materialized) == 0)
        attributes = (ancestorIndex.Attributes | IndexAttributes.Virtual | IndexAttributes.Union) &
                     ~(IndexAttributes.Real | IndexAttributes.Join | IndexAttributes.Filtered);
      else
        attributes = (ancestorIndex.Attributes | IndexAttributes.Real) 
          & ~(IndexAttributes.Join | IndexAttributes.Union | IndexAttributes.Filtered 
          | IndexAttributes.Virtual | IndexAttributes.Abstract);
      if(isAbstract)
        attributes = attributes | IndexAttributes.Abstract;
      FillFactor = ancestorIndex.FillFactor;
      shortName = ancestorIndex.ShortName;
      declaringIndex = ancestorIndex.DeclaringIndex;
    }

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true"/>
    /// </summary>
    /// <param name="reflectedType">Reflected type.</param>
    /// <param name="indexAttributes">The index attributes.</param>
    /// <param name="baseIndex">Base index.</param>
    /// <param name="baseIndexes">The base indexes.</param>
    public IndexInfo(TypeInfo reflectedType, IndexAttributes indexAttributes, IndexInfo baseIndex, params IndexInfo[] baseIndexes)
      : this (baseIndex.DeclaringType, baseIndex, false)
    {
      attributes = baseIndex.Attributes &
                   ~(IndexAttributes.Abstract | IndexAttributes.Join 
                   | IndexAttributes.Union | IndexAttributes.Filtered | IndexAttributes.Real) |
                   indexAttributes | IndexAttributes.Virtual;
      UnderlyingIndexes.Add(baseIndex);
      foreach (IndexInfo info in baseIndexes)
        UnderlyingIndexes.Add(info);
      declaringIndex = baseIndex.DeclaringIndex;
      this.reflectedType = reflectedType;
    }
  }
}
