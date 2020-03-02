using System.Linq;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace LinqPatcher.Basics.Analyzer
{
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
            var ffa = LastOperator.OperatorType.ReturnType();
            var methodReturnType = LastOperator.NestedMethod.ReturnType;
            return methodReturnType;
        }
    }
}