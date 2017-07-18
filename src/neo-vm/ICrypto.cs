namespace Neo.VM
{
    /// <summary>
    /// Performs basic hashing and message authentication.
    /// </summary>
    public interface ICrypto
    {
        byte[] Hash160(byte[] message);

        byte[] Hash256(byte[] message);

        bool VerifySignature(byte[] message, byte[] signature, byte[] pubkey);
    }
}
