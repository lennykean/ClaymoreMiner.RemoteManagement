using Newtonsoft.Json;

namespace ClaymoreMiner.RemoteManagement.Models
{
    /// <summary>
    /// Mining pool statistics
    /// </summary>
    public class PoolStats
    {
        /// <summary>
        /// Initializes a new <see cref="PoolStats" />
        /// </summary>
        [JsonConstructor]
        public PoolStats(string pool, int hashRate, int shares, int rejectedShares, int invalidShares, int poolSwitches)
        {
            Pool = pool;
            Hashrate = hashRate;
            Shares = shares;
            RejectedShares = rejectedShares;
            InvalidShares = invalidShares;
            PoolSwitches = poolSwitches;
        }
        /// <summary>
        /// Mining pool address.
        /// </summary>
        public string Pool { get; }
        /// <summary>
        /// Total hashrate.
        /// </summary>
        public int Hashrate { get; }
        /// <summary>
        /// Total shares.
        /// </summary>
        public int Shares { get; }
        /// <summary>
        /// Number of rejected shares.
        /// </summary>
        public int RejectedShares { get; }
        /// <summary>
        /// Number of invalid shares.
        /// </summary>
        public int InvalidShares { get; }
        /// <summary>
        /// Number of pool switches.
        /// </summary>
        public int PoolSwitches { get; }
    }
}