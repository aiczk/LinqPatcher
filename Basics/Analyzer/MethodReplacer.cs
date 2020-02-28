using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinqPatcher.Basics.Analyzer
{
    public class MethodReplacer
    {
        private MethodBody methodBody;

        private Instruction nop;
        private Instruction ldfld;
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

                if (opCode == OpCodes.Nop)
                {
                    var next = instruction.Next;

                    if (next.OpCode == OpCodes.Ldarg_0)
                    {
                        nop = instruction;
                    }
                }

                //field
                if (opCode == OpCodes.Ldarg_0)
                {
                    var next = instruction.Next;

                    if (next.OpCode == OpCodes.Ldfld)
                    {
                        flag = true;
                        ldfld = next;
                    }
                }

                //new
//                if (opCode == OpCodes.Ldloc_0 || opCode == OpCodes.Ldloc_1 || 
//                    opCode == OpCodes.Ldloc_2 || opCode == OpCodes.Ldloc_3 ||
//                    opCode == OpCodes.Ldloc_S)
//                {
//                    var next = instruction.Next;
//
//                    if (next.OpCode != OpCodes.Ldsfld)
//                    {
//                        flag = true;
//                    }
//                }

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

        public void Replace(MethodDefinition callMethod)
        {
            var processor = methodBody.GetILProcessor();

            processor.InsertBefore(nop, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(nop, Instruction.Create(OpCodes.Ldarg_0));
            processor.InsertBefore(nop, ldfld);
            processor.InsertBefore(nop, Instruction.Create(OpCodes.Call, callMethod));
            processor.InsertBefore(nop, stLoc);
        }
        
    }

    public enum CallType
    {
        Field,
        Argument,
        Local
    }
}