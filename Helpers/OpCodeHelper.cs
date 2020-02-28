using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinqPatcher.Helpers
{
    public static class OpCodeHelper
    {
        public static OpCode LdcI4(int length)
        {
            switch (length)
            {
                case 0:
                    return OpCodes.Ldc_I4_0;
                case 1:
                    return OpCodes.Ldc_I4_1;
                case 2:
                    return OpCodes.Ldc_I4_2;
                case 3:
                    return OpCodes.Ldc_I4_3;
                case 4:
                    return OpCodes.Ldc_I4_4;
                case 5:
                    return OpCodes.Ldc_I4_5;
                case 6:
                    return OpCodes.Ldc_I4_6;
                case 7:
                    return OpCodes.Ldc_I4_7;
                case 8:
                    return OpCodes.Ldc_I4_8;
                case var len when len <= sbyte.MaxValue && len >= sbyte.MinValue:
                    return OpCodes.Ldc_I4_S;
                default:
                    return OpCodes.Ldc_I4;
            }
        }
        
        public static OpCode LdLoc(int index)
        {
            switch (index)
            {
                case 0:
                    return OpCodes.Ldloc_0;
                case 1:
                    return OpCodes.Ldloc_1;
                case 2:
                    return OpCodes.Ldloc_2;
                case 3:
                    return OpCodes.Ldloc_3;
                default:
                    return OpCodes.Ldloc_S;
            }
        }

        public static OpCode StLoc(int index)
        {
            switch (index)
            {
                case 0:
                    return OpCodes.Stloc_0;
                case 1:
                    return OpCodes.Stloc_1;
                case 2:
                    return OpCodes.Stloc_2;
                case 3:
                    return OpCodes.Stloc_3;
                default:
                    return OpCodes.Stloc_S;
            }
        }
        
        public static OpCode LdElem(TypeReference arg)
        {
            switch (arg.Name)
            {
                case nameof(SByte):
                    return OpCodes.Ldelem_I1;
                case nameof(Int16):
                    return OpCodes.Ldelem_I2;
                case nameof(Int32):
                    return OpCodes.Ldelem_I4;
                case nameof(Int64):
                    return OpCodes.Ldelem_I8;
                case nameof(Byte):
                    return OpCodes.Ldelem_U1;
                case nameof(UInt16):
                    return OpCodes.Ldelem_U2;
                case nameof(UInt32):
                    return OpCodes.Ldelem_U4;
                case nameof(Single):
                    return OpCodes.Ldelem_R4;
                case nameof(Double):
                    return OpCodes.Ldelem_R8;
                default:
                    return OpCodes.Ldelem_Ref;
            }
        }

        public static OpCode LdArg(int argIndex)
        {
            
            switch (argIndex)
            {
                case 0:
                    return OpCodes.Ldarg_0;
                case 1:
                    return OpCodes.Ldarg_1;
                case 2:
                    return OpCodes.Ldarg_2;
                case 3:
                    return OpCodes.Ldarg_3;
                default:
                    return OpCodes.Ldarg_S;
            }
        }
    }
}