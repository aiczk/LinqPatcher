﻿using System;
using System.Collections.Generic;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Basics.Operator;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using UnityEngine;

namespace LinqPatcher.Basics.Analyzer
{
    public class MethodAnalyzer
    {
        private ModuleDefinition coreModule;
        private TypeSystem typeSystem;

        public MethodAnalyzer(ModuleDefinition coreModule)
        {
            this.coreModule = coreModule;
            typeSystem = coreModule.TypeSystem;
        }

        public AnalyzedMethod Analyze(MethodDefinition method)
        {
            MethodDefinition nestedMethodToken = null;
            var operatorType = OperatorType.None;

            var operators = new Collection<LinqOperator>();

            //返り値にできるっぽい型はキャッシュしておく
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

                    if (operatorMethodToken == null)
                        continue;

                    operatorType = (OperatorType) Enum.Parse(typeof(OperatorType), operatorMethodToken.Name);
                }

                var linqOperator = new LinqOperator(nestedMethodToken, operatorType);
                operators.Add(linqOperator);
            }

            return new AnalyzedMethod(coreModule, operators.ToReadOnlyCollection());
        }

        private static T GetToken<T>(Instruction instruction) where T : class => instruction.Operand as T;

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
                
                case OperatorType.ToList:
                    op = null;
                    break;
                
                default:
                    throw new NotSupportedException($"{linqOperator.OperatorType.ToString()} is not supported.");
            }

            return op;
        }
    }
}