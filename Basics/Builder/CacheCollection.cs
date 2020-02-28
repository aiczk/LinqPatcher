using System.Linq;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace LinqPatcher.Basics.Builder
{
    public class CacheCollection
    {
        public Instruction Add { get;}
        
        private TypeSystem typeSystem;
        private ModuleDefinition systemModule;
        private ModuleDefinition mainModule;

        private FieldDefinition list;
        private GenericInstanceType iEnumerable;

        private MethodReference ctor;
        private MethodReference add;
        private MethodReference getCount;
        private MethodReference clear;

        public CacheCollection(ModuleDefinition systemModule, ModuleDefinition mainModule)
        {
            this.systemModule = systemModule;
            this.mainModule = mainModule;
            typeSystem = systemModule.TypeSystem;
            
            var methods = systemModule.GetType("System.Collections.Generic", "List`1").Methods;
            ctor = methods.Single(x => x.Name == ".ctor" && x.Parameters.Count == 0);
            add = methods.Single(x => x.Name == "Add");
            getCount = methods.Single(x => x.Name == "get_Count");
            clear = methods.Single(x => x.Name == "Clear");

            Add = Instruction.Create(OpCodes.Ldarg_0);
        }

        public void Constructor(TypeDefinition targetClass, TypeReference argument)
        {
            //todo .ctorがない場合
            var classCtor = targetClass.Methods.Single(x => x.Name == ".ctor");
            var methodBody = classCtor.Body;
            
            ctor = mainModule.ImportReference(ctor.Resolve().MakeGeneric(argument));

            var first = methodBody.Instructions.First();
            var processor = methodBody.GetILProcessor();
            
            processor.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Newobj, ctor));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Stfld, list));
        }

        public void InitField(TypeDefinition targetClass, string fieldName, TypeReference argument)
        {
            var listInstance = Import("System.Collections.Generic", "List`1").MakeGenericInstanceType(argument);
            iEnumerable = Import("System.Collections.Generic", "IEnumerable`1").MakeGenericInstanceType(argument);

            add = mainModule.ImportReference(add.Resolve().MakeGeneric(argument));
            getCount = mainModule.ImportReference(getCount.Resolve().MakeGeneric(argument));
            clear = mainModule.ImportReference(clear.Resolve().MakeGeneric(argument));

            list = new FieldDefinition(fieldName, FieldAttributes.Private, listInstance);
            targetClass.Fields.Add(list);
        }

        public void Define(MethodBody methodBody)
        {
            var boolean = methodBody.AddVariableDefinition(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();
            var jumpLabel = Instruction.Create(OpCodes.Nop);
            
            //if(collection.Count < 0)
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, list);
            
            //get_count()
            processor.Emit(OpCodes.Callvirt, getCount);
            processor.Emit(OpCodes.Ldc_I4_0);
            processor.Emit(OpCodes.Cgt);
            processor.Append(InstructionHelper.StLoc(boolean));
            processor.Append(InstructionHelper.LdLoc(boolean));
            processor.Emit(OpCodes.Brfalse_S, jumpLabel);
            
            //collection.Clear();
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, list);
            
            //clear()
            processor.Emit(OpCodes.Callvirt, clear);
            
            processor.Emit(OpCodes.Nop);
            processor.Append(jumpLabel);
        }

        public void AddValue(MethodBody methodBody, VariableDefinition local)
        {
            var processor = methodBody.GetILProcessor();
            
            processor.Append(Add);
            processor.Emit(OpCodes.Ldfld, list);
            processor.Append(InstructionHelper.LdLoc(local));
            processor.Emit(OpCodes.Callvirt, add);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Nop);
        }

        public void ReturnValue(MethodBody methodBody)
        {
            var variable = methodBody.AddVariableDefinition(iEnumerable);
            var processor = methodBody.GetILProcessor();
            
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, list);
            processor.Append(InstructionHelper.StLoc(variable));

            var fa = InstructionHelper.LdLoc(variable);
            processor.Emit(OpCodes.Br_S, fa);
            processor.Append(fa);
        }

        private TypeReference Import(string nameSpace, string name)
        {
            var type = systemModule.GetType(nameSpace, name);
            var result = mainModule.ImportReference(type);
            
            return result;
        }
    }
}