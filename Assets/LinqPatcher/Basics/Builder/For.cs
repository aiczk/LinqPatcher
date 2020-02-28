using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinqPatcher.Basics.Builder
{
    public class For
    {
        public Instruction IncrementIndex { get; private set; }
        public VariableDefinition LocalDefinition { get; set; }
        
        private VariableDefinition indexDefinition;
        private Instruction loopStart;
        private Instruction loopEnd;
        private Instruction loopCheck;
        private TypeSystem typeSystem;

        public For(TypeSystem typeSystem)
        {
            this.typeSystem = typeSystem;
            
            loopStart = Instruction.Create(OpCodes.Nop);
            loopEnd = Instruction.Create(OpCodes.Nop);
        }

        public void Start(MethodBody methodBody, int initValue = 0)
        {
            indexDefinition = methodBody.AddVariableDefinition(typeSystem.Int32);
            loopCheck = InstructionHelper.LdLoc(indexDefinition);
            IncrementIndex = InstructionHelper.LdLoc(indexDefinition);
            var processor = methodBody.GetILProcessor();

            //i = n
            processor.Append(InstructionHelper.LdcI4(initValue));
            processor.Append(InstructionHelper.StLoc(indexDefinition));
            
            //i < n check
            processor.Emit(OpCodes.Br_S, loopCheck);
            
            //loop start
            processor.Append(loopStart);
        }
        
        public void End(MethodBody methodBody)
        {
            var withInVariable = methodBody.AddVariableDefinition(typeSystem.Boolean);
            var processor = methodBody.GetILProcessor();

            //loop end
            processor.Append(loopEnd);
            
            //i++
            processor.Append(IncrementIndex);
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Add);
            processor.Append(InstructionHelper.StLoc(indexDefinition));
            
            //i < arr.Length
            processor.Append(loopCheck);
            
            //arr.Length
            processor.Append(InstructionHelper.LdArg(1));
            processor.Emit(OpCodes.Ldlen);
            processor.Emit(OpCodes.Conv_I4);
            
            processor.Emit(OpCodes.Clt);
            processor.Append(InstructionHelper.StLoc(withInVariable));
            
            //check within range
            processor.Append(InstructionHelper.LdLoc(withInVariable));
            processor.Emit(OpCodes.Brtrue_S, loopStart);
        }

        public void DefineLocal(MethodBody methodBody, TypeReference argType)
        { 
            LocalDefinition = methodBody.AddVariableDefinition(argType);
            var processor = methodBody.GetILProcessor();
            
            processor.Append(InstructionHelper.LdArg(1));
            processor.Append(InstructionHelper.LdLoc(indexDefinition));
            processor.Append(InstructionHelper.LdElem(argType));
            processor.Append(InstructionHelper.StLoc(LocalDefinition));
        }
    }
}
