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

        private MethodReference listCtor;
        private MethodReference add;
        private MethodReference getCount;
        private MethodReference clear;

        public CacheCollection(ModuleDefinition systemModule, ModuleDefinition mainModule)
        {
            this.systemModule = systemModule;
            this.mainModule = mainModule;
            typeSystem = systemModule.TypeSystem;
            
            var listMethods = systemModule.GetType("System.Collections.Generic", "List`1").Methods;
            listCtor = listMethods.Single(x => x.Name == ".ctor" && x.Parameters.Count == 0);
            add = listMethods.Single(x => x.Name == "Add");
            getCount = listMethods.Single(x => x.Name == "get_Count");
            clear = listMethods.Single(x => x.Name == "Clear");

            Add = Instruction.Create(OpCodes.Ldarg_0);
        }

        public void Create(TypeDefinition targetClass, string fieldName, TypeReference argument)
        {
            var listInstance = ImportAsGeneric("System.Collections.Generic", "List`1", argument);
            iEnumerable = ImportAsGeneric("System.Collections.Generic", "IEnumerable`1", argument);

            add = mainModule.ImportReference(add.MakeGeneric(argument));
            getCount = mainModule.ImportReference(getCount.MakeGeneric(argument));
            clear = mainModule.ImportReference(clear.MakeGeneric(argument));

            list = new FieldDefinition(fieldName, FieldAttributes.Private, listInstance);
            targetClass.Fields.Add(list);

            InitList(targetClass, argument);
        }
        
        private void InitList(TypeDefinition targetClass, TypeReference argument)
        {
            if (!targetClass.HasConstructors(out var ctor)) 
                ctor = targetClass.DefineConstructor(typeSystem);
            
            listCtor = mainModule.ImportReference(listCtor.MakeGeneric(argument));

            var ctorBody = ctor.Body;
            var first = ctorBody.Instructions.First();
            var processor = ctorBody.GetILProcessor();
            
            processor.InsertBefore(first, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Newobj, listCtor));
            processor.InsertBefore(first, Instruction.Create(OpCodes.Stfld, list));
        }

        public void Define(MethodBody methodBody)
        {
            var boolean = methodBody.AddVariable(typeSystem.Boolean);
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
            
            //list.add(local);
            processor.Emit(OpCodes.Callvirt, add);
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Nop);
        }

        public void ReturnValue(MethodBody methodBody)
        {
            var variable = methodBody.AddVariable(iEnumerable);
            var processor = methodBody.GetILProcessor();
            
            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Ldfld, list);
            processor.Append(InstructionHelper.StLoc(variable));

            var loadEnumerable = InstructionHelper.LdLoc(variable);
            processor.Emit(OpCodes.Br_S, loadEnumerable);
            processor.Append(loadEnumerable);
        }

        private GenericInstanceType ImportAsGeneric(string nameSpace, string name, TypeReference argument)
        {
            var type = systemModule.GetType(nameSpace, name);
            var result = mainModule.ImportReference(type).MakeGenericInstanceType(argument);
            
            return result;
        }
    }
}