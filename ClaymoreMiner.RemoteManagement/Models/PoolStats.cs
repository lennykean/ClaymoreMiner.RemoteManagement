using Newtonsoft.Json;

namespace ClaymoreMiner.RemoteManagement.Models
{
    public class PoolStats
    {
        [JsonConstructor]
        public PoolStats(string pool, int hashRate, int shares, int invalidShares, int rejectedShares, int poolSwitches)
        {
            Pool = pool;
            Hashrate = hashRate;
            Shares = shares;
            RejectedShares = rejectedShares;
            InvalidShares = invalidShares;
            PoolSwitches = poolSwitches;
        }
        public string Pool { get; }
        public int Hashrate { get; }
        public int Shares { get; }
        public int RejectedShares { get; }
        public int InvalidShares { get; }
        public int PoolSwitches { get; }
    }
}