namespace ClaymoreMiner.RemoteManagement.Models
{
    /// <summary>
    /// GPU mode
    /// </summary>
    public enum GpuMode
    {
        /// <summary>
        /// Disabled
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Ethereum only mining
        /// </summary>
        EthereumOnly = 1,
        /// <summary>
        /// Dual mining
        /// </summary>
        Dual = 2
    }
}