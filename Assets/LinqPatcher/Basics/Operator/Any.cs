using LinqPatcher.Basics.Builder;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinqPatcher.Basics.Operator
{
    public class Any : ILinqOperator
    {
        private TypeSystem typeSystem;
        private MethodBody funcMethod;
        private For forLoop;
        private Instruction[] converted;
        
        JumpType ILinqOperator.Type => JumpType.Jump;

        public Any(TypeSystem typeSystem, MethodDefinition funcMethod, For forLoop)
        {
            this.typeSystem = typeSystem;
            this.funcMethod = funcMethod.Body;
            this.forLoop = forLoop;
        }
        
        Instruction ILinqOperator.Next()
        {
            converted = InstructionHelper.ConvertFunction(funcMethod, forLoop);

            return converted[0];
        }

        void ILinqOperator.Define(MethodBody method, Instruction jumpInstruction)
        {
            var resultVariable = method.AddVariable(typeSystem.Boolean);
            var processor = method.GetILProcessor();
            
            converted = converted ?? InstructionHelper.ConvertFunction(funcMethod, forLoop);

            foreach (var instruction in converted)
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(resultVariable));
            processor.Append(InstructionHelper.LdLoc(resultVariable));
            
            //if(expression)
            processor.Emit(OpCodes.Brfalse_S, jumpInstruction);
            
            //result = true;
            processor.Emit(OpCodes.Nop);
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Append(InstructionHelper.StLoc(resultVariable));
            
            //break;
            //jump to *return result*
            processor.Emit(OpCodes.Br_S, );
        }
    }
}