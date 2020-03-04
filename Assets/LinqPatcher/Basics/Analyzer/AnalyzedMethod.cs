using System.Linq;
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

        private ModuleDefinition mainModule;
        private TypeReference parameterType;
        private TypeReference returnType;

        public AnalyzedMethod(ModuleDefinition mainModule, ReadOnlyCollection<LinqOperator> operators)
        {
            this.mainModule = mainModule;
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
            var type = lastOperator.OperatorType.TypeOf();
            
            var method = new TypeReference(type.Namespace, type.Name, ModuleDefinition.ReadModule(type.Assembly.Location), mainModule);
            method.GenericParameters.Add(new GenericParameter(lastOperator.NestedMethod.ReturnType));
            var genericMethod = method.MakeGenericInstanceType(lastOperator.NestedMethod.ReturnType);
            var importedMethod = mainModule.ImportReference(genericMethod);

            return importedMethod;
        }
        
    }
}