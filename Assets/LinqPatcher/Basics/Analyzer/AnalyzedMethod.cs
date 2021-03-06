﻿using System.Linq;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using UnityEngine;

namespace LinqPatcher.Basics.Analyzer
{
    public class AnalyzedMethod
    {
        public TypeReference ParameterType => parameterType = parameterType ?? GetParameterType();
        public TypeReference ReturnType => returnType = returnType ?? GetReturnType();
        public ReadOnlyCollection<LinqOperator> Operators { get; }

        private ModuleDefinition coreModule;
        private TypeReference parameterType;
        private TypeReference returnType;

        public AnalyzedMethod(ModuleDefinition coreModule, ReadOnlyCollection<LinqOperator> operators)
        {
            this.coreModule = coreModule;
            Operators = operators;
        }

        private TypeReference GetParameterType()
        {
            var firstOperator = Operators.First();
            var parameterDefinition = firstOperator.NestedMethod.Parameters[0];
            return parameterDefinition.ParameterType;
        }
        
        //todo correspond to the non-generic.
        private TypeReference GetReturnType()
        {
            var lastOperator = Operators.Last(x => x.OperatorType.IsSupportedOperator());
            var type = lastOperator.OperatorType.ReturnType();
            
            var method = new TypeReference(type.Namespace, type.Name, ModuleDefinition.ReadModule(type.Assembly.Location), coreModule);
            method.GenericParameters.Add(new GenericParameter(lastOperator.NestedMethod.ReturnType));
            var generic = method.MakeGenericInstanceType(lastOperator.NestedMethod.ReturnType);

            return generic;
        }
        
    }
}