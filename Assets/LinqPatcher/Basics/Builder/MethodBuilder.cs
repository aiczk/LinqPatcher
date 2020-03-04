﻿using System;
using System.Collections.Generic;
using LinqPatcher.Basics.Analyzer;
using LinqPatcher.Basics.Operator;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace LinqPatcher.Basics.Builder
{
    public class MethodBuilder
    {
        internal For MainLoop { get; }

        private ModuleDefinition mainModule;
        private MethodBody methodBody;
        private CacheCollection cacheCollection;
        private Queue<ILinqOperator> operators;
        private TypeReference paramType;
        private MethodDefinition method;
        private Arg arg;

        public MethodBuilder(ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            this.mainModule = mainModule;
            cacheCollection = new CacheCollection(systemModule, mainModule);
            operators = new Queue<ILinqOperator>();
            MainLoop = new For(systemModule.TypeSystem);
            arg = new Arg();
        }

        public void Create(TypeDefinition targetClass, string methodName, TypeReference paramsType, TypeReference returnType)
        {
            method = new MethodDefinition(methodName, MethodAttributes.Private, returnType);
            targetClass.Methods.Add(method);
            
            arg.Define(method.Body, paramsType);
            
            //todo 返り値がEnumerableなら定義する。
            cacheCollection.Create(targetClass, $"linq_{methodName}", ((GenericInstanceType)returnType).GenericArguments[0]);

            methodBody = method.Body;
            paramType = paramsType;
        }

        public void Begin()
        {
            cacheCollection.Define(methodBody);
            MainLoop.Start(methodBody);
            MainLoop.DefineLocal(methodBody, paramType);
        }

        public void End()
        {
            cacheCollection.AddValue(methodBody, MainLoop.LocalDefinition);
            MainLoop.End(methodBody);
            cacheCollection.ReturnValue(methodBody);
            InstructionHelper.Return(methodBody);
        }

        public void AppendOperator(ILinqOperator linqOperator) => operators.Enqueue(linqOperator);

        public void BuildOperator()
        {
            var count = operators.Count;
            for (var i = 0; i < count; i++)
            {
                var linqOperator = operators.Dequeue();
                
                if (linqOperator.Type == JumpType.Jump)
                {
                    var nextOperator = operators.Count > 0 ? operators.Peek() : null;
                    var nextProcess = nextOperator == null ? cacheCollection.Add : nextOperator.Next();
                    
                    linqOperator.Define(methodBody, nextProcess);
                    continue;
                }
                
                linqOperator.Define(methodBody, null);
            }
        }

        public void Replace(MethodDefinition targetMethod)
        {
            var replacer = new MethodReplacer(targetMethod.Body);
            replacer.RemoveSection();
            replacer.Replace(method);
        }
    }
}