using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ArxOne.MrAdvice
{
    /// <summary>
    /// Stolen from http://stackoverflow.com/a/1821857
    /// </summary>
    public static class ILReader
    {
        private static readonly Dictionary<short, OpCode> OpCodeList = typeof(OpCodes).GetFields()
            .Where(f => f.FieldType == typeof(OpCode))
            .Select(f => (OpCode)f.GetValue(null))
            .ToDictionary(o => o.Value);

        /// <summary>
        /// Reads the il code from given method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IEnumerable<EmittedOpCode> ReadIL(this MethodBase method)
        {
            var body = method.GetMethodBody();
            if (body == null)
                yield break;

            var module = method.Module;
            var offset = 0;
            byte[] il = body.GetILAsByteArray();
            while (offset < il.Length)
            {
                var startOffset = offset;
                var opCodeByte = il[offset];
                short opCodeValue = opCodeByte;
                offset++;

                // If it's an extended opcode then grab the second byte. The 0xFE prefix codes aren't marked as prefix operators though.
                if (opCodeValue == 0xFE || OpCodeList[opCodeValue].OpCodeType == OpCodeType.Prefix)
                {
                    opCodeValue = (short)((opCodeValue << 8) + il[offset]);
                    offset++;
                }

                var code = OpCodeList[opCodeValue];

                object argument = null;

                var argumentSize = 4;
                switch (code.OperandType)
                {
                    case OperandType.InlineNone:
                        argumentSize = 0;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        argumentSize = 1;
                        break;
                    case OperandType.InlineVar:
                        argumentSize = 2;
                        break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        argumentSize = 8;
                        break;
                    case OperandType.InlineSwitch:
                        long num = il[offset] + (il[offset + 1] << 8) + (il[offset + 2] << 16) + (il[offset + 3] << 24);
                        argumentSize = (int)(4 * num + 4);
                        break;
                }

                // This does not currently handle the 'switch' instruction meaningfully.
                if (argumentSize > 0)
                {
                    long arg = 0;
                    for (int i = 0; i < argumentSize; ++i)
                    {
                        long v = il[offset + i];
                        arg += v << (i * 8);
                    }
                    argument = arg;
                    offset += argumentSize;

                    if (code == OpCodes.Ldfld || code == OpCodes.Ldflda || code == OpCodes.Ldsfld || code == OpCodes.Ldsflda || code == OpCodes.Stfld)
                        argument = module.ResolveField((int)arg);
                    else if (code == OpCodes.Call || code == OpCodes.Calli || code == OpCodes.Callvirt)
                        argument = module.ResolveMethod((int)arg);
                    else if (code == OpCodes.Newobj)
                        // This displays the type whose constructor is being called, but you can also determine the specific constructor and find out about its parameter types
                        argument = module.ResolveMethod((int)arg);
                    else if (code == OpCodes.Ldtoken)
                        argument = module.ResolveMember((int)arg);
                    else if (code == OpCodes.Ldstr)
                        argument = module.ResolveString((int)arg);
                    else if (code == OpCodes.Constrained || code == OpCodes.Box)
                        argument = module.ResolveType((int)arg);
                    else
                        argument = arg;

                }


                yield return new EmittedOpCode(startOffset, code, argument);
            }
        }
    }
}