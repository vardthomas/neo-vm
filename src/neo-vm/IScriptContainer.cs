namespace Neo.VM
{
    /// <summary>
    /// Allows callers to get an message. This is often used along with a public key and signture 
    /// to verifying message authenticity.
    /// </summary>
    /// <seealso cref="OpCode.CHECKSIG"/>
    /// <seealso cref="OpCode.CHECKMULTISIG"/>
    public interface IScriptContainer : IInteropInterface
    {
        byte[] GetMessage();
    }
}
