// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Yakunin
// Created:    2009.03.16

using System;
using System.Diagnostics;
using Xtensive.Modelling.Resources;

namespace Xtensive.Modelling
{
  /// <summary>
  /// Abstract base class for node with specified <see cref="Node.Model"/> 
  /// and <see cref="Node.Parent"/> types.
  /// </summary>
  /// <typeparam name="TParent">The type of the parent.</typeparam>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  [Serializable]
  public abstract class Node<TParent, TModel> : Node,
    INode<TParent>
    where TParent : Node
    where TModel : Model
  {
    /// <summary>
    /// Gets the parent node.
    /// </summary>
    public new TParent Parent { 
      [DebuggerStepThrough]
      get { return base.Parent as TParent; }
    }

    /// <summary>
    /// Gets the model this node belongs to.
    /// </summary>
    public new TModel Model {
      [DebuggerStepThrough]
      get { return base.Model as TModel; }
    }

    /// <inheritdoc/>
    public void Move(TParent newParent, string newName, int newIndex)
    {
      Move((Node) newParent, newName, newIndex);
    }


    // Constructors

    /// <inheritdoc/>
    protected Node(TParent parent, string name, int index)
      : base(parent, name, index)
    {
    }

    /// <inheritdoc/>
    protected Node(Node parent, string name)
      : base(parent, name)
    {
    }
  }
}