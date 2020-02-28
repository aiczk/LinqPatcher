using Mono.Cecil.Cil;

namespace LinqPatcher.Basics.Operator
{
    public interface ILinqOperator
    {
        JumpType Type { get; }
        void Define(MethodBody method, Instruction jumpInstruction);
        Instruction Next();
    }
}