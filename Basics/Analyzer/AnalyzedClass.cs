using Mono.Cecil;
using Mono.Collections.Generic;

namespace LinqPatcher.Basics.Analyzer
{
    public class AnalyzedClass
    {
        public ReadOnlyCollection<TypeDefinition> OptimizeTypes { get; }

        public AnalyzedClass(ReadOnlyCollection<TypeDefinition> optimizeTypes)
        {
            OptimizeTypes = optimizeTypes;
        }
    }
}