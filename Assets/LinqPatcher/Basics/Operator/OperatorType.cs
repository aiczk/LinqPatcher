﻿namespace LinqPatcher.Basics.Operator
{
    public enum ReturnValueType
    {
        Enumerable,
        Primitive
    }
    
    public enum OperatorType
    {
        None = -1,
        
        //Supported.
        Where = 0,
        Select,
        OrderBy,
        OrderByDescending,
        GroupBy,
        ToList,
        SequenceEqual,
        All,
        Any,
        Contains,
        Count,
        LongCount,
        Average,
        
        //Not Supported.
        First,
        FirstOrDefault,
        Last,
        LastOrDefault,
        Single,
        SingleOrDefault,
        ElementAt,
        ElementAtOrDefault,
        Distinct,
        Union,
        Intersect,
        Except,
        AsEnumerable,
        ToArray,
        ThenByDescending,
        Reverse,
        ToDictionary,
        ToLookUp,
        OfType,
        Cast,
        SelectMany,
        Take,
        Skip,
        TakeWhile,
        SkipWhile,
        Join,
        GroupJoin,
        Concat,
        DefaultIfEmpty,
        Aggregate,
        Sum,
        Min,
        Max,
    }
}