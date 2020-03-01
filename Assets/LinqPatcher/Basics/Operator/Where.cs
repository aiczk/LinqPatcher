using LinqPatcher.Basics.Builder;
using LinqPatcher.Helpers;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinqPatcher.Basics.Operator
{
    public class Where : ILinqOperator
    {
        private For forLoop;
        private MethodBody funcMethod;
        private TypeSystem typeSystem;
        private Instruction[] converted;
        JumpType ILinqOperator.Type => JumpType.Jump;
        
        public Where(TypeSystem typeSystem, MethodDefinition funcMethod, For forLoop)
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
            var checkVariable = method.AddVariable(typeSystem.Boolean);
            var processor = method.GetILProcessor();
            
            converted = converted ?? InstructionHelper.ConvertFunction(funcMethod, forLoop);
            
            foreach (var instruction in converted)
            {
                processor.Append(instruction);
            }
            
            processor.Append(InstructionHelper.StLoc(checkVariable));
            processor.Append(InstructionHelper.LdLoc(checkVariable));
            
            //true
            processor.Emit(OpCodes.Brfalse_S, jumpInstruction);
            //continue
            processor.Emit(OpCodes.Br_S, forLoop.IncrementIndex);
        }
    }
}