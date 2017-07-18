using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.VM
{
    /// <summary>
    /// Builds a byte representation of a script.
    /// <remarks>
    /// This class is heavily used in the Neo GUI wallet. It is used to process <see cref="OpCode"/>'s
    /// and emit a corresponding byte array. Internally, the opcodes are stored in the <see cref="ms"/> field.
    /// </remarks>
    /// </summary>
    public class ScriptBuilder : IDisposable
    {
        private MemoryStream ms = new MemoryStream();

        /// <summary>
        /// Gets the current offset for the memory stream.
        /// </summary>
        public int Offset => (int)ms.Position;

        public void Dispose()
        {
            ms.Dispose();
        }

        /// <summary>
        /// Writes the byte representation of the OpCode into the <see cref="ms"/> field.
        /// If the <see cref="arg"/> property is not null, it will write it immedatiately after the opcode.
        /// </summary>
        /// <param name="op">The OpCode to emit.</param>
        /// <param name="arg">Optional byte array that can be used to pass in arguments along with the OpCode.</param>
        public ScriptBuilder Emit(OpCode op, byte[] arg = null)
        {
            ms.WriteByte((byte)op);
            if (arg != null)
                ms.Write(arg, 0, arg.Length);
            return this;
        }

        /// <summary>
        /// Emits a call to a script.
        /// </summary>
        /// <param name="scriptHash"></param>
        /// <param name="useTailCall"></param>
        /// <returns></returns>
        public ScriptBuilder EmitAppCall(byte[] scriptHash, bool useTailCall = false)
        {
            if (scriptHash.Length != 20)
                throw new ArgumentException();
            return Emit(useTailCall ? OpCode.TAILCALL : OpCode.APPCALL, scriptHash);
        }

        /// <summary>
        /// Emits a jump to a new offset.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ScriptBuilder EmitJump(OpCode op, short offset)
        {
            if (op != OpCode.JMP && op != OpCode.JMPIF && op != OpCode.JMPIFNOT && op != OpCode.CALL)
                throw new ArgumentException();
            return Emit(op, BitConverter.GetBytes(offset));
        }

        /// <summary>
        /// Emits push operation with a correspending <see cref="BigInteger"/>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public ScriptBuilder EmitPush(BigInteger number)
        {
            if (number == -1) return Emit(OpCode.PUSHM1);
            if (number == 0) return Emit(OpCode.PUSH0);
            if (number > 0 && number <= 16) return Emit(OpCode.PUSH1 - 1 + (byte)number);
            return EmitPush(number.ToByteArray());
        }

        /// <summary>
        /// If data is true, emits 1, else emits 0
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ScriptBuilder EmitPush(bool data)
        {
            return Emit(data ? OpCode.PUSHT : OpCode.PUSHF);
        }

        /// <summary>
        /// First emits the length of <see cref="data"/>, then pushes its contents onto the stack
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ScriptBuilder EmitPush(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();
            if (data.Length <= (int)OpCode.PUSHBYTES75)
            {
                ms.WriteByte((byte)data.Length);
                ms.Write(data, 0, data.Length);
            }
            else if (data.Length < 0x100)
            {
                Emit(OpCode.PUSHDATA1);
                ms.WriteByte((byte)data.Length);
                ms.Write(data, 0, data.Length);
            }
            else if (data.Length < 0x10000)
            {
                Emit(OpCode.PUSHDATA2);
                ms.Write(BitConverter.GetBytes((ushort)data.Length), 0, 2);
                ms.Write(data, 0, data.Length);
            }
            else// if (data.Length < 0x100000000L)
            {
                Emit(OpCode.PUSHDATA4);
                ms.Write(BitConverter.GetBytes((uint)data.Length), 0, 4);
                ms.Write(data, 0, data.Length);
            }
            return this;
        }

        /// <summary>
        /// Emits a system call.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public ScriptBuilder EmitSysCall(string api)
        {
            if (api == null)
                throw new ArgumentNullException();
            byte[] api_bytes = Encoding.ASCII.GetBytes(api);
            if (api_bytes.Length == 0 || api_bytes.Length > 252)
                throw new ArgumentException();
            byte[] arg = new byte[api_bytes.Length + 1];
            arg[0] = (byte)api_bytes.Length;
            Buffer.BlockCopy(api_bytes, 0, arg, 1, api_bytes.Length);
            return Emit(OpCode.SYSCALL, arg);
        }



        /// <summary>
        /// Returns byte representation of the current program
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return ms.ToArray();
        }
    }
}
