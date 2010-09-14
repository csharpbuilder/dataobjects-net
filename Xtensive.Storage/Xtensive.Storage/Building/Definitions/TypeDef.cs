// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2007.08.27

using System;
using System.Diagnostics;
using System.Reflection;
using Xtensive.Core;
using Xtensive.Core.Reflection;
using Xtensive.Storage.Building.Builders;
using Xtensive.Storage.Model;
using TypeAttributes=Xtensive.Storage.Model.TypeAttributes;

namespace Xtensive.Storage.Building.Definitions
{
  /// <summary>
  /// Defines a single persistent type.
  /// </summary>
  [DebuggerDisplay("{underlyingType}")]
  [Serializable]
  public sealed class TypeDef : MappingNode
  {
    private readonly Type underlyingType;
    private TypeAttributes attributes;
    private readonly NodeCollection<FieldDef> fields;
    private readonly NodeCollection<IndexDef> indexes;
    private NodeCollection<TypeDef> implementors;
  
    /// <summary>
    /// Gets a value indicating whether this instance is entity.
    /// </summary>
    public bool IsEntity
    {
      get { return (attributes & TypeAttributes.Entity) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is abstract entity.
    /// </summary>
    public bool IsAbstract
    {
      get { return (attributes & TypeAttributes.Abstract) > 0; }
      internal set {
        this.EnsureNotLocked();
        Attributes = value
          ? (Attributes | TypeAttributes.Abstract)
          : (Attributes & ~TypeAttributes.Abstract);
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is system type.
    /// </summary>
    public bool IsSystem
    {
      get { return (attributes & TypeAttributes.System) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is interface.
    /// </summary>
    public bool IsInterface
    {
      get { return (attributes & TypeAttributes.Interface) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is structure.
    /// </summary>
    public bool IsStructure
    {
      get { return (attributes & TypeAttributes.Structure) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is generic type definition.
    /// </summary>
    public bool IsGenericTypeDefinition
    {
      get { return (attributes & TypeAttributes.GenericTypeDefinition) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is automatically registered generic type instance.
    /// </summary>
    public bool IsAutoGenericInstance
    {
      get { return (attributes & TypeAttributes.AutoGenericInstance) > 0; }
    }

    /// <summary>
    /// Gets or sets the underlying system type.
    /// </summary>
    public Type UnderlyingType
    {
      get { return underlyingType; }
    }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    public TypeAttributes Attributes
    {
      get { return attributes; }
      internal set { attributes = value; }
    }

    /// <summary>
    /// Gets the indexes for this instance.
    /// </summary>
    public NodeCollection<IndexDef> Indexes
    {
      get { return indexes; }
    }

    /// <summary>
    /// Gets the fields contained in this instance.
    /// </summary>
    public NodeCollection<FieldDef> Fields
    {
      get { return fields; }
    }

    /// <summary>
    /// Gets the direct implementors of this instance (if this is an interface).
    /// </summary>
    public NodeCollection<TypeDef> Implementors
    {
      get { return implementors; }
      internal set { implementors = value; }
    }

    /// <summary>
    /// Gets or sets the type discriminator value.
    /// </summary>
    /// <value>The type discriminator value.</value>
    public object TypeDiscriminatorValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is default type in hierarchy.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this instance is default type in hierarchy; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsDefaultTypeInHierarchy { get; set; }

    /// <summary>
    /// Defines the index and adds it to the <see cref="Indexes"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Argument "name" is invalid.</exception>
    public IndexDef DefineIndex(string name)
    {
      Validator.ValidateName(name, ValidationRule.Index);

      var indexDef = new IndexDef {Name = name, IsSecondary = true};
      indexes.Add(indexDef);
      return indexDef;
    }

    /// <summary>
    /// Defines the field and adds it to the <see cref="Fields"/>.
    /// </summary>
    /// <param name="property">The underlying property.</param>
    /// <returns></returns>
    public FieldDef DefineField(PropertyInfo property)
    {
      ArgumentValidator.EnsureArgumentNotNull(property, "property");

      if (property.ReflectedType != UnderlyingType)
        throw new DomainBuilderException(
          string.Format(Resources.Strings.ExPropertyXMustBeDeclaredInTypeY, property.Name, UnderlyingType.GetFullName()));
            
      FieldDef fieldDef = ModelDefBuilder.DefineField(property);
      fields.Add(fieldDef);
      return fieldDef;
    }

    /// <summary>
    /// Defines the field and adds it to the <see cref="Fields"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="valueType">The type of the field value.</param>
    /// <returns></returns>
    public FieldDef DefineField(string name, Type valueType)
    {
      ArgumentValidator.EnsureArgumentNotNull(valueType, "type");
      ArgumentValidator.EnsureArgumentNotNullOrEmpty(name, "name");

      FieldDef field = ModelDefBuilder.DefineField(UnderlyingType, name, valueType);
      fields.Add(field);
      return field;
    }

    /// <summary>
    /// Performs additional custom processes before setting new name to this instance.
    /// </summary>
    /// <param name="newName">The new name of this instance.</param>
    protected override void ValidateName(string newName)
    {
      base.ValidateName(newName);
      Validator.ValidateName(newName, ValidationRule.Type);
    }


    // Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeDef"/> class.
    /// </summary>
    /// <param name="type">The underlying type.</param>
    internal TypeDef(Type type)
    {
      underlyingType = type;
      if (type.IsInterface)
        Attributes = TypeAttributes.Interface;
      else if (type == typeof (Structure) || type.IsSubclassOf(typeof (Structure)))
        Attributes = TypeAttributes.Structure;
      else
        Attributes = type.IsAbstract
          ? TypeAttributes.Entity | TypeAttributes.Abstract
          : TypeAttributes.Entity;
      if (type.IsGenericTypeDefinition)
        Attributes = Attributes | TypeAttributes.GenericTypeDefinition;
      fields = new NodeCollection<FieldDef>(this, "Fields");
      indexes = new NodeCollection<IndexDef>(this, "Indexes");
      implementors = IsInterface 
        ? new NodeCollection<TypeDef>(this, "Implementors") 
        : NodeCollection<TypeDef>.Empty;
    }
  }
}