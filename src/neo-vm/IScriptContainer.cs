namespace Neo.VM
{
    /// <summary>
    /// Allows callers to get a message. This is often used along with a public key and signature 
    /// to verify message authenticity.
    /// </summary>
    /// <seealso cref="OpCode.CHECKSIG"/>
    /// <seealso cref="OpCode.CHECKMULTISIG"/>
    public interface IScriptContainer : IInteropInterface
    {
        byte[] GetMessage();
    }
}
