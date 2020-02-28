using LinqPatcher.Basics.Operator;
using Mono.Cecil;

namespace LinqPatcher.Basics.Builder
{
    public class LinqOperator
    {
        public OperatorType OperatorType { get; }
        public MethodDefinition NestedMethod { get; }
        
        public LinqOperator(MethodDefinition nestedMethod, OperatorType operatorType)
        {
            NestedMethod = nestedMethod;
            OperatorType = operatorType;
        }
    }
}