using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.VM
{
    /// <summary>
    /// Establishes a context for execution of scripts.
    /// </summary>
    /// <remarks>
    /// Whenever a script is executed, a new instance <see cref="ExecutionContext"/> is pushed onto the stack.
    /// Its main job is to track the current instruction pointer along w/ breakpoints.
    /// If the <see cref="PushOnly"/> flag is set to true, any opcode other than push instructions will put <see cref="engine"/>
    /// in a <see cref="VMState.FAULT"/> state.
    /// </remarks>
    public class ExecutionContext : IDisposable
    {
        private ExecutionEngine engine;
        public readonly byte[] Script;
        public readonly bool PushOnly;
        internal readonly BinaryReader OpReader;
        internal readonly HashSet<uint> BreakPoints;

        /// <summary>
        /// Keeps track of current IP
        /// </summary>
        public int InstructionPointer
        {
            get
            {
                return (int)OpReader.BaseStream.Position;
            }
            set
            {
                OpReader.BaseStream.Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Get the next instruction to be executed
        /// </summary>
        public OpCode NextInstruction => (OpCode)Script[OpReader.BaseStream.Position];

        private byte[] _script_hash = null;
        public byte[] ScriptHash
        {
            get
            {
                if (_script_hash == null)
                    _script_hash = engine.Crypto.Hash160(Script);
                return _script_hash;
            }
        }

        internal ExecutionContext(ExecutionEngine engine, byte[] script, bool push_only, HashSet<uint> break_points = null)
        {
            this.engine = engine;
            this.Script = script;
            this.PushOnly = push_only;
            this.OpReader = new BinaryReader(new MemoryStream(script, false));
            this.BreakPoints = break_points ?? new HashSet<uint>();
        }

        public ExecutionContext Clone()
        {
            return new ExecutionContext(engine, Script, PushOnly, BreakPoints)
            {
                InstructionPointer = InstructionPointer
            };
        }

        public void Dispose()
        {
            OpReader.Dispose();
        }
    }
}
