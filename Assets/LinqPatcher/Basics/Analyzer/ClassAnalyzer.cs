using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace LinqPatcher.Basics.Analyzer
{
    public class ClassAnalyzer
    {
        private ModuleDefinition moduleDefinition;
        private TypeDefinition attribute;

        public ClassAnalyzer(ModuleDefinition moduleDefinition, TypeDefinition attribute)
        {
            this.moduleDefinition = moduleDefinition;
            this.attribute = attribute;
        }

        public ReadOnlyCollection<MethodDefinition> AnalyzeMethod(TypeDefinition typeDefinition)
        {
            var methods = new Collection<MethodDefinition>();
            var attributeName = attribute.Name;
            foreach (var methodDefinition in typeDefinition.Methods)
            {
                if (!methodDefinition.HasCustomAttributes)
                    continue;

                foreach (var customAttribute in methodDefinition.CustomAttributes)
                {
                    var attributeType = customAttribute.AttributeType;

                    if (attributeType.Name != attributeName)
                        continue;
                        
                    methods.Add(methodDefinition);
                    break;
                }
            }

            return methods.ToReadOnlyCollection();
        }
        
        public AnalyzedClass Analyze()
        {
            var classes = new Collection<TypeDefinition>();
            var attributeName = attribute.Name;
            
            foreach (var classDefinition in moduleDefinition.Types)
            {
                if(!classDefinition.IsClass)
                    continue;
                
                if(!classDefinition.HasMethods)
                    continue;
                
                if(!classDefinition.HasNestedTypes)
                    continue;

                foreach (var methodDefinition in classDefinition.Methods)
                {
                    if(!CheckAttribute(methodDefinition,attributeName))
                        continue;
                    
                    classes.Add(classDefinition);
                    break;
                }
            }
            
            return new AnalyzedClass(classes.ToReadOnlyCollection());
        }
        
        private bool CheckAttribute(MethodDefinition method, string attributeName)
        {
            if (!method.HasCustomAttributes)
                return false;
            
            var find = false;
            foreach (var customAttribute in method.CustomAttributes)
            {
                var attributeType = customAttribute.AttributeType;

                if (attributeType.Name != attributeName) 
                    continue;
                
                find = true;
                break;
            }
            
            return find;
        }
    }
}