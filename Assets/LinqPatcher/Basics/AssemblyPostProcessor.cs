using LinqPatcher.Basics.Analyzer;
using LinqPatcher.Basics.Builder;
using LinqPatcher.Helpers;
using Mono.Cecil;
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
            AssemblyHelper.Executer(TargetModuleName, () =>
            {
                var readerParam = AssemblyHelper.ReadAndWrite();
                var mainModule = AssemblyHelper.FindModule(TargetModuleName, readerParam);
                var l2MModule = AssemblyHelper.FindModule("LinqPatcherAttribute", readerParam);
                var coreModule = AssemblyHelper.GetCoreModule();
                PostCompile(mainModule, l2MModule, coreModule);
            });
        }

        private static void PostCompile(ModuleDefinition mainModule, ModuleDefinition l2MModule, ModuleDefinition coreModule)
        {
            var l2MOptimizeAttribute = l2MModule.GetType("LinqPatcher.Attributes", "OptimizeAttribute");
            
            var classAnalyzer = new ClassAnalyzer(mainModule, l2MOptimizeAttribute);
            var methodAnalyzer = new MethodAnalyzer(mainModule);
            var methodBuilder = new MethodBuilder(mainModule, coreModule);
            
            var analyzedClass = classAnalyzer.Analyze();
            foreach (var optimizeClass in analyzedClass.OptimizeTypes)
            {
                foreach (var method in classAnalyzer.AnalyzeMethod(optimizeClass))
                {
                    var analyzedMethod = methodAnalyzer.Analyze(method);
                    
                    methodBuilder.Create(optimizeClass, MethodHelper.CreateUniqueName, analyzedMethod.ParameterType, analyzedMethod.ReturnType);
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
