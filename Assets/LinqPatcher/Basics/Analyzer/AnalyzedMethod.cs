using System.Linq;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Basics.Operator;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace LinqPatcher.Basics.Analyzer
{
    public class AnalyzedMethod
    {
        public TypeReference ParameterType => parameterType = parameterType ?? GetArgType();
        public TypeReference ReturnType => returnType = returnType ?? GetReturnType();
        public ReadOnlyCollection<LinqOperator> Operators { get; }

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
            //todo ハードコーディングを改善する。
            //select以外にもあるはず。
            var lastOperator = Operators.Last(x => x.OperatorType == OperatorType.Select);
            var methodReturnType = lastOperator.NestedMethod.ReturnType;
            return methodReturnType;
        }
    }
}