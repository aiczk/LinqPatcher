using LinqPatcher.Basics.Operator;
using Mono.Cecil;

namespace LinqPatcher.Basics.Builder
{
    public class LinqOperator
    {
        public Operator.OperatorType OperatorType { get; }
        public MethodDefinition NestedMethod { get; }
        
        public LinqOperator(MethodDefinition nestedMethod, Operator.OperatorType operatorType)
        {
            NestedMethod = nestedMethod;
            OperatorType = operatorType;
        }
    }
}