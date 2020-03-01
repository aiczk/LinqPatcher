using System;
using System.Collections.Generic;
using System.Linq;
using LinqPatcher.Basics.Operator;

namespace LinqPatcher.Helpers
{
    public static class OperatorHelper
    {
        public static bool IsGenerics(this OperatorType operatorType)
        {
             switch (operatorType)
            {
                case OperatorType.Select:
                case OperatorType.OrderBy:
                case OperatorType.OrderByDescending:
                case OperatorType.GroupBy:
                case OperatorType.ToList:
                    return true;

                case OperatorType.Any:
                case OperatorType.All:
                case OperatorType.Contains:
                case OperatorType.SequenceEqual:
                case OperatorType.Count:
                case OperatorType.LongCount:
                case OperatorType.Average:
                    return true;

                default:
                    throw new NotSupportedException("This operator is not supported.");
       
//todo support
//                case OperatorType.ToArray:
//                    break;
//                case OperatorType.ToDictionary:
//                    break;
//                case OperatorType.ToLookUp:
//                    break;
//                case OperatorType.First:
//                case OperatorType.FirstOrDefault:
//                case OperatorType.Last:
//                case OperatorType.LastOrDefault:
//                case OperatorType.Single:
//                case OperatorType.SingleOrDefault:
//                case OperatorType.ElementAt:
//                case OperatorType.ElementAtOrDefault:
//                    break;
//                case OperatorType.Sum:
//                    break;
//                case OperatorType.Min:
//                    break;
//                case OperatorType.Max:
//                    break;
//                case OperatorType.Aggregate:
//                    break;
            }
        }
        
        //todo generic
        public static Type ReturnType(this OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.Select:
                    return typeof(IEnumerable<>);
                
                case OperatorType.OrderBy:
                case OperatorType.OrderByDescending:
                    return typeof(IOrderedEnumerable<>);
                
                case OperatorType.GroupBy:
                    return typeof(IEnumerable<>);

                case OperatorType.ToList:
                    return typeof(List<>);

                case OperatorType.Any:
                case OperatorType.All:
                case OperatorType.Contains:
                case OperatorType.SequenceEqual:
                    return typeof(bool);
                
                case OperatorType.Count:
                    return typeof(int);
                
                case OperatorType.LongCount:
                    return typeof(long);

                case OperatorType.Average:
                    return typeof(decimal);

                default:
                    throw new NotSupportedException("This operator is not supported.");
       
//todo support
//                case OperatorType.ToArray:
//                    break;
//                case OperatorType.ToDictionary:
//                    break;
//                case OperatorType.ToLookUp:
//                    break;
//                case OperatorType.First:
//                case OperatorType.FirstOrDefault:
//                case OperatorType.Last:
//                case OperatorType.LastOrDefault:
//                case OperatorType.Single:
//                case OperatorType.SingleOrDefault:
//                case OperatorType.ElementAt:
//                case OperatorType.ElementAtOrDefault:
//                    break;
//                case OperatorType.Sum:
//                    break;
//                case OperatorType.Min:
//                    break;
//                case OperatorType.Max:
//                    break;
//                case OperatorType.Aggregate:
//                    break;
            }
        }

        public static bool IsSupportedOperator(this OperatorType operatorType)
        {
            var index = (int)operatorType;

            return index > -1 && index <= 13;
        }
    }
}