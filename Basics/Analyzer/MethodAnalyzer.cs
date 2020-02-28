using System;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Basics.Operator;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace LinqPatcher.Basics.Analyzer
{
    public class MethodAnalyzer
    {
        private TypeSystem typeSystem;

        public MethodAnalyzer(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
        }

        public AnalyzedMethod Analyze(MethodDefinition method)
        {
            MethodDefinition nestedMethodToken = null;
            OperatorType operatorTypeType = OperatorType.None;
            
            var operators = new Collection<LinqOperator>();

            foreach (var instruction in method.Body.Instructions)
            {
                var opCode = instruction.OpCode;

                if (opCode != OpCodes.Ldftn && opCode != OpCodes.Call)
                    continue;

                if (opCode == OpCodes.Ldftn)
                {
                    nestedMethodToken = GetToken<MethodDefinition>(instruction);
                    continue;
                }

                if (opCode == OpCodes.Call)
                {
                    var operatorMethodToken = GetToken<GenericInstanceMethod>(instruction);
                    
                    if(operatorMethodToken == null)
                        continue;
                    
                    operatorTypeType = (OperatorType) Enum.Parse(typeof(OperatorType), operatorMethodToken.Name);
                }

                if (nestedMethodToken == null || operatorTypeType == OperatorType.None)
                    continue;

                var linqOperator = new LinqOperator(nestedMethodToken, operatorTypeType);
                operators.Add(linqOperator);

                nestedMethodToken = null;
                operatorTypeType = OperatorType.None;
            }

            return new AnalyzedMethod(operators.ToReadOnlyCollection());

            T GetToken<T>(Instruction instruction) where T : class
            {
                return instruction.Operand as T;
            }
        }

        public ILinqOperator OperatorFactory(LinqOperator linqOperator, MethodBuilder methodBuilder)
        {
            ILinqOperator op;
            switch (linqOperator.OperatorType)
            {
                case OperatorType.Where:
                    op = new Where(typeSystem, linqOperator.NestedMethod, methodBuilder.MainLoop);
                    break;
                
                case OperatorType.Select:
                    op = new Select(linqOperator.NestedMethod, methodBuilder.MainLoop);
                    break;
                
                default:
                    op = null;
                    break;
            }

            return op;
        }
    }
}