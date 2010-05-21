// Copyright (C) 2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexis Kochetov
// Created:    2010.04.07

using System;
using System.Linq;
using System.Reflection;
using PostSharp.AspectInfrastructure;
using PostSharp.AspectWeaver;
using PostSharp.AspectWeaver.AspectWeavers;
using PostSharp.AspectWeaver.Transformations;
using PostSharp.CodeModel;
using PostSharp.CodeWeaver;
using PostSharp.Extensibility;
using Xtensive.Core.Aspects;
using Xtensive.Core.Reflection;

namespace Xtensive.Core.Weaver
{
  internal class ImplementConstructorWeaver : TypeLevelAspectWeaver
  {
    private ImplementConstructorTransformation transformation;

    protected override void Initialize()
    {
      base.Initialize();

      transformation = new ImplementConstructorTransformation(this);
      ApplyEffectWaivers(transformation);
      RequiresRuntimeInstance = false;
      RequiresRuntimeInstanceInitialization = false;
      RequiresRuntimeReflectionObject = false;
    }

    protected override AspectWeaverInstance CreateAspectWeaverInstance(AspectInstanceInfo aspectInstanceInfo)
    {
      return new Instance(this, aspectInstanceInfo);
    }


    // Constructors

    public ImplementConstructorWeaver()
      : base(null, MulticastTargets.Class)
    { }


    // Nested class

    private class Instance : TypeLevelAspectWeaverInstance
    {
      private readonly ImplementConstructorWeaver parent;

      public override void ProvideAspectTransformations(AspectWeaverTransformationAdder adder)
      {
        adder.Add(TargetElement, parent.transformation.CreateInstance(this));
      }

      public Instance(ImplementConstructorWeaver parent, AspectInstanceInfo aspectInstanceInfo)
        : base(parent, aspectInstanceInfo)
      {
        this.parent = parent;
      }
    }
  }

  internal class ImplementConstructorTransformation : StructuralTransformation
  {
    public override string GetDisplayName(MethodSemantics semantic)
    {
      return "Implementing constructor";
    }

    public AspectWeaverTransformationInstance CreateInstance(AspectWeaverInstance aspectWeaverInstance)
    {
      var module = AspectWeaver.Module;
      var aspect = (ImplementConstructor)aspectWeaverInstance.Aspect;
      var argumentTypes = aspect.ParameterTypes.Select(t => module.Cache.GetType(t)).ToArray();
      return new Instance(this, aspectWeaverInstance, argumentTypes);
    }


    // Constructors

    public ImplementConstructorTransformation(AspectWeaver aspectWeaver)
      : base(aspectWeaver)
    {
    }

    // Nested class

    private class Instance : StructuralTransformationInstance
    {
      private const string ParameterNamePrefix = "arg";
      private readonly ITypeSignature[] argumentTypes;


      public override void Implement(StructuralTransformationContext context)
      {
        var typeDef = (TypeDefDeclaration)context.TargetElement;
        var baseType = typeDef.BaseType;
        var module = AspectWeaver.Module;
        var helper = new WeavingHelper(module);
        var ctorDef = new MethodDefDeclaration();
        ctorDef.Name = WellKnown.CtorName;
        ctorDef.CallingConvention = CallingConvention.HasThis;
        ctorDef.Attributes = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        typeDef.Methods.Add(ctorDef);
        ctorDef.CustomAttributes.Add(helper.GetCompilerGeneratedAttribute());
        ctorDef.ReturnParameter = new ParameterDeclaration();
        ctorDef.ReturnParameter.Name = string.Empty;
        ctorDef.ReturnParameter.ParameterType = module.Cache.GetIntrinsic(IntrinsicType.Void);
        ctorDef.ReturnParameter.Attributes = ParameterAttributes.Retval;
  
        for (int i = 0; i < argumentTypes.Length; i++)
          ctorDef.Parameters.Add(new ParameterDeclaration(i, ParameterNamePrefix + i, argumentTypes[i].TranslateType(module)));
        ctorDef.MethodBody = new MethodBodyDeclaration();
        ctorDef.MethodBody.RootInstructionBlock = ctorDef.MethodBody.CreateInstructionBlock();
        IMethod baseConstructor = null;

        var ctorSignature =
          new MethodSignature(
            argumentTypes[0].Module,
            CallingConvention.HasThis,
            module.Cache.GetIntrinsic(IntrinsicType.Void),
            argumentTypes,
            0);

        try {
          baseConstructor = baseType.Methods.GetMethod(WellKnown.CtorName,
           ctorSignature.Translate(module),
           BindingOptions.Default).TranslateMethod(module);
        } catch (Exception e) {
          ErrorLog.Write(SeverityType.Error, "{0}", e);
          return;
        }

        var sequence = ctorDef.MethodBody.CreateInstructionSequence();
        ctorDef.MethodBody.RootInstructionBlock.AddInstructionSequence(sequence, PostSharp.Collections.NodePosition.After, null);
        using (var writer = new InstructionWriter()) {
          writer.AttachInstructionSequence(sequence);
          writer.EmitInstruction(OpCodeNumber.Ldarg_0);
          for (short i = 0; i < argumentTypes.Length; i++)
            writer.EmitInstructionParameter(OpCodeNumber.Ldarg_S, ctorDef.Parameters[i]);

          writer.EmitInstructionMethod(OpCodeNumber.Call, baseConstructor);
          writer.EmitInstruction(OpCodeNumber.Ret);
          writer.DetachInstructionSequence();
        }
      }


      // Constructors

      public Instance(ImplementConstructorTransformation parent, AspectWeaverInstance aspectWeaverInstance, ITypeSignature[] argumentTypes)
        : base(parent, aspectWeaverInstance)
      {
        this.argumentTypes = argumentTypes;
      }
    }
  }

}