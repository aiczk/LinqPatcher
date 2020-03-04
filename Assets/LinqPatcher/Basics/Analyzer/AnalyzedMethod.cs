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
        public TypeReference ParameterType => parameterType = parameterType ?? GetParameterType();
        public TypeReference ReturnType => returnType = returnType ?? GetReturnType();
        public LinqOperator LastOperator => lastOperator = lastOperator ?? Operators.Last(x => x.OperatorType.IsSupportedOperator());
        public ReadOnlyCollection<LinqOperator> Operators { get; }

        private ModuleDefinition coreModule;
        private LinqOperator lastOperator;
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
        
        private TypeReference GetReturnType()
        {
            //IEnumerable<>の状態
//            var type = LastOperator.OperatorType.ReturnType();
//            var tr = new TypeReference(type.Namespace, type.Name, ModuleDefinition.ReadModule(type.Assembly.Location), coreModule);
            
            var methodReturnType = LastOperator.NestedMethod.ReturnType;
            return methodReturnType;
        }
    }
}