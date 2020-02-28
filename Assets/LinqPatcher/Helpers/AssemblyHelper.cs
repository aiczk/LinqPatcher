using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace LinqPatcher.Helpers
{
    public static class AssemblyHelper
    {
        public static ReaderParameters ReadAndWrite() => 
            new ReaderParameters
        {
            ReadWrite = true,
            InMemory = true,
            ReadingMode = ReadingMode.Immediate
        };

        public static bool ExistMainAssembly() => AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "Main");

        public static ModuleDefinition FindModule(string assemblyName, ReaderParameters readerParameters)
        {
            var assembly = FindAssembly(assemblyName);
            return AssemblyDefinition.ReadAssembly(assembly.Location, readerParameters).MainModule;
        }

        private static Assembly FindAssembly(string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.First(x => x.GetName().Name == assemblyName);
            return assembly;
        }
        
        public static ModuleDefinition GetCoreModule() => ModuleDefinition.ReadModule(typeof(object).Module.FullyQualifiedName);
    }
}
