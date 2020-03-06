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
            var lastReturnType = lastOperator.OperatorType.TypeOf();

            var module = ModuleDefinition.ReadModule(lastReturnType.Assembly.Location);
            var methodReference = new TypeReference(lastReturnType.Namespace, lastReturnType.Name, module, mainModule);

            if (lastReturnType.IsGenericType)
            {
                var nestedMethodReturnType = lastOperator.NestedMethod.ReturnType;
                methodReference.GenericParameters.Add(new GenericParameter(nestedMethodReturnType));
                methodReference = methodReference.MakeGenericInstanceType(nestedMethodReturnType);
            }
            
            var importedMethod = mainModule.ImportReference(methodReference);

            return importedMethod;
        }
        
    }
}