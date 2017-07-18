using System;

namespace Neo.VM
{
    /// <summary>
    /// Keeps track of the current state of the executing virtual machine.
    /// <remarks> This is mostly used within the <see cref="ExecutionEngine"/> class.</remarks>
    /// </summary>
    [Flags]
    public enum VMState : byte
    {
        NONE = 0,

        HALT = 1 << 0,
        FAULT = 1 << 1,
        BREAK = 1 << 2,
    }
}
