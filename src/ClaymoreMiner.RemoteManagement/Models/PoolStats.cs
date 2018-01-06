namespace ClaymoreMiner.RemoteManagement.Models
{
    /// <summary>
    /// Mining pool statistics
    /// </summary>
    public class PoolStats
    {
        /// <summary>
        /// Mining pool address.
        /// </summary>
        public string Pool { get; set; }
        /// <summary>
        /// Total hashrate.
        /// </summary>
        public int Hashrate { get; set; }
        /// <summary>
        /// Total shares.
        /// </summary>
        public int Shares { get; set; }
        /// <summary>
        /// Number of rejected shares.
        /// </summary>
        public int RejectedShares { get; set; }
        /// <summary>
        /// Number of invalid shares.
        /// </summary>
        public int InvalidShares { get; set; }
        /// <summary>
        /// Number of pool switches.
        /// </summary>
        public int PoolSwitches { get; set; }
    }
}