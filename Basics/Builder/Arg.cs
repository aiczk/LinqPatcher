using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinqPatcher.Basics.Builder
{
    public class Arg
    {
        public void Define(MethodBody methodBody, TypeReference arrayType)
        {
            methodBody.Method.Parameters.Add(new ParameterDefinition(new ArrayType(arrayType)));
        }
    }
}
