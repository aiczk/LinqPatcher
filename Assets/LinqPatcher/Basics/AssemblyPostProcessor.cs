using System;
using LinqPatcher.Basics.Analyzer;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Basics.Operator;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using UnityEditor;
using UnityEngine;

namespace LinqPatcher.Basics
{
    [InitializeOnLoad]
    internal static class AssemblyPostProcessor
    {
        private static readonly string TargetModuleName = "Main";
        
        static AssemblyPostProcessor()
        { 
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            
            PostCompile();
        }

        private static void PostCompile()
        {
            if(!AssemblyHelper.ExistMainAssembly())
                return;
            
            EditorApplication.LockReloadAssemblies();
            try
            {
                var readerParams = AssemblyHelper.ReadAndWrite();
                var mainModule = AssemblyHelper.FindModule(TargetModuleName, readerParams);
                var l2MModule = AssemblyHelper.FindModule("LinqPatcherAttribute", readerParams);
                var coreModule = AssemblyHelper.GetCoreModule();
                Execute(mainModule, l2MModule, coreModule);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void Execute(ModuleDefinition mainModule, ModuleDefinition l2MModule, ModuleDefinition coreModule)
        {
            var l2MOptimizeAttribute = l2MModule.GetType("LinqPatcher.Attributes", "OptimizeAttribute");
            
            var classAnalyzer = new ClassAnalyzer(mainModule, l2MOptimizeAttribute);
            var methodAnalyzer = new MethodAnalyzer(coreModule);
            var methodBuilder = new MethodBuilder(mainModule, coreModule);
            
            var analyzedClass = classAnalyzer.Analyze();
            foreach (var optimizeClass in analyzedClass.OptimizeTypes)
            {
                foreach (var method in classAnalyzer.AnalyzeMethod(optimizeClass))
                {
                    var analyzedMethod = methodAnalyzer.Analyze(method);
                    
                    var returnType = mainModule.ImportReference(analyzedMethod.ReturnType);

                    methodBuilder.Create(optimizeClass, MethodHelper.CreateUniqueName, analyzedMethod.ParameterType, returnType);
                    methodBuilder.Begin();
                    
                    foreach (var linqOperator in analyzedMethod.Operators)
                    {
                        var linq = methodAnalyzer.OperatorFactory(linqOperator, methodBuilder);
                        methodBuilder.AppendOperator(linq);
                    }
                    
                    methodBuilder.BuildOperator();
                    methodBuilder.End();
                    methodBuilder.Replace(method);
                }
            }

            mainModule.Write($"{TargetModuleName}.dll");
        }
    }
}
