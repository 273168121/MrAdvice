#region Mr. Advice
// Mr. Advice
// A simple post build weaving package
// http://mradvice.arxone.com/
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.MrAdvice
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// An <see cref="OpCode"/> with its argument
    /// </summary>
    public class EmittedOpCode
    {
        /// <summary>
        ///     Gets the start offset.
        /// </summary>
        /// <value>
        ///     The start offset.
        /// </value>
        public int StartOffset { get; }

        /// <summary>
        ///     Gets the op code.
        /// </summary>
        /// <value>
        ///     The op code.
        /// </value>
        public OpCode OpCode { get; }

        /// <summary>
        ///     Gets the argument.
        /// </summary>
        /// <value>
        ///     The argument.
        /// </value>
        public object Argument { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmittedOpCode" /> class.
        /// </summary>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="opCode">The op code.</param>
        /// <param name="argument">The argument.</param>
        public EmittedOpCode(int startOffset, OpCode opCode, object argument)
        {
            StartOffset = startOffset;
            OpCode = opCode;
            Argument = argument;
        }

        public override string ToString()
        {
            return OpCode + (Argument == null ? String.Empty : " " + Argument);
        }
    }
}