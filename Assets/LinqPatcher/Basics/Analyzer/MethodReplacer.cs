using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Debug = UnityEngine.Debug;

namespace LinqPatcher.Basics.Analyzer
{
    public class MethodReplacer
    {
        private MethodBody methodBody;

        private Instruction nop;
        private Instruction ldArray;
        private Instruction stLoc;
        
        public MethodReplacer(MethodBody methodBody)
        {
            this.methodBody = methodBody;
        }
        
        public void RemoveSection()
        {
            var list = new List<Instruction>();
            var flag = false;
            foreach (var instruction in methodBody.Instructions)
            {
                var opCode = instruction.OpCode;
                
                if(nop == null)
                    FindNop(instruction);
                
                //field
                if (flag == false && Field(instruction))
                    flag = true;

                //arg
                if (flag == false && Arg(instruction))
                    flag = true;
                
                if (opCode == OpCodes.Stloc_0 || opCode == OpCodes.Stloc_1 ||
                    opCode == OpCodes.Stloc_2 || opCode == OpCodes.Stloc_3 ||
                    opCode == OpCodes.Stloc_S)
                {
                    var previous = instruction.Previous;

                    if (previous.OpCode == OpCodes.Call)
                    {
                        flag = false;
                        stLoc = instruction;
                        list.Add(instruction);
                    }
                }
                
                if(!flag)
                    continue;
                
                list.Add(instruction);
            }

            foreach (var instruction in list)
            {
                methodBody.Instructions.Remove(instruction);
            }
        }

        private void FindNop(Instruction instruction)
        {
            var opCode = instruction.OpCode;
            if (opCode != OpCodes.Nop)
                return;
            
            var next = instruction.Next;

            if (next.OpCode != OpCodes.Ldarg_0 &&
                next.OpCode != OpCodes.Ldarg_1 && next.OpCode != OpCodes.Ldarg_2 &&
                next.OpCode != OpCodes.Ldarg_3 && next.OpCode != OpCodes.Ldarg_S) 
                return;
            
            nop = instruction;
        }

        private bool Field(Instruction instruction)
        {
            if (instruction.OpCode != OpCodes.Ldarg_0)
                return false;
            
            var next = instruction.Next;

            if (next.OpCode != OpCodes.Ldfld)
                return false;
            
            ldArray = next;
            return true;
        }
        
        private bool Arg(Instruction instruction)
        {
            var opCode = instruction.OpCode;
            if (opCode != OpCodes.Ldarg_1 && opCode != OpCodes.Ldarg_2 &&
                opCode != OpCodes.Ldarg_3 && opCode != OpCodes.Ldarg_S) 
                return false;
            
            var next = instruction.Next;
            if (next.OpCode != OpCodes.Ldsfld)
                return false;

            ldArray = instruction;
            return true;

        }

        public void Replace(MethodDefinition callMethod)
        {
            var processor = methodBody.GetILProcessor();

            processor.InsertBefore(nop, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(nop, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(nop, ldArray);
            processor.InsertBefore(nop, Instruction.Create(OpCodes.Call, callMethod));
            processor.InsertBefore(nop, stLoc);
        }
    }
}