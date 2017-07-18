namespace Neo.VM
{
    /// <summary>
    /// Defines a way for callers to get Scripts based on its script hash.
    /// </summary>
    public interface IScriptTable
    {
        byte[] GetScript(byte[] script_hash);
    }
}
