using System.Linq;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace LinqPatcher.Basics.Analyzer
{
    public class MethodSignature
    {
        public TypeReference ReturnType { get; }
        public TypeReference GenericParameter { get; }
        public bool IsEnumerable { get; }

        public MethodSignature(ModuleDefinition mainModule, AnalyzedMethod analyzedMethod)
        {
            GenericParameter = analyzedMethod.ReturnType;

            var lastOperator = analyzedMethod.LastOperator.OperatorType;
            var type = lastOperator.ReturnType();
            ReturnType = mainModule.ImportReference(type).MakeGenericInstanceType(analyzedMethod.ReturnType);
            IsEnumerable = lastOperator.IsGenerics();
        }
    }

    public class AnalyzedMethod
    {
        public TypeReference ParameterType => parameterType = parameterType ?? GetArgType();
        public TypeReference ReturnType => returnType = returnType ?? GetReturnType();
        public LinqOperator LastOperator => lastOperator = lastOperator ?? Operators.Last(x => x.OperatorType.IsSupportedOperator());
        public ReadOnlyCollection<LinqOperator> Operators { get; }
        
        private LinqOperator lastOperator;
        private TypeReference parameterType;
        private TypeReference returnType;

        public AnalyzedMethod(ReadOnlyCollection<LinqOperator> operators)
        {
            Operators = operators;
        }

        private TypeReference GetArgType()
        {
            var firstOperator = Operators.First();
            var parameterDefinition = firstOperator.NestedMethod.Parameters[0];
            return parameterDefinition.ParameterType;
        }
        
        private TypeReference GetReturnType()
        {
            var methodReturnType = LastOperator.NestedMethod.ReturnType;
            return methodReturnType;
        }
    }
}