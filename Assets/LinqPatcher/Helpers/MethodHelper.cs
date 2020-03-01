using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace LinqPatcher.Helpers
{
    public static class MethodHelper
    {
        public static string CreateUniqueName => Guid.NewGuid().ToString("N");
        
        public static VariableDefinition AddVariable(this MethodBody methodBody, TypeReference reference)
        {
            var variable = new VariableDefinition(reference);
            methodBody.Variables.Add(variable);
            return variable;
        }
        
        public static MethodReference MakeGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            self = self.Resolve();
            var reference = new MethodReference(self.Name, self.ReturnType)
            {
                DeclaringType = self.DeclaringType.MakeGenericInstanceType(arguments),
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention,
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var genericParameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));

            return reference;
        }
        
        public static bool HasConstructors(this TypeDefinition typeDefinition, out MethodDefinition ctor)
        {
            ctor = null;
            var result = false;
            foreach (var method in typeDefinition.Methods)
            {
                if(method.Name != ".ctor")
                    continue;

                ctor = method;
                result = true;
                break;
            }

            return result;
        }

        public static MethodDefinition DefineConstructor(this TypeDefinition typeDefinition, TypeSystem typeSystem)
        {
            var ctor = new MethodDefinition
            (
                ".ctor",
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName |
                MethodAttributes.Public,
                typeSystem.Void
            );
            
            typeDefinition.Methods.Add(ctor);

            return ctor;
        }
    }
}