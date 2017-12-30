using System;
using System.Collections.Immutable;

namespace ClaymoreMiner.RemoteManagement.Models
{
    public class MinerStatistics
    {
        public MinerStatistics(string minerVersion, TimeSpan uptime, PoolStats ethereum, PoolStats decred, ImmutableList<GpuStats> gpus)
        {
            MinerVersion = minerVersion;
            Uptime = uptime;
            Ethereum = ethereum;
            Decred = decred;
            Gpus = gpus;
        }
        public string MinerVersion { get; }
        public TimeSpan Uptime { get; }
        public PoolStats Ethereum { get; }
        public PoolStats Decred { get; }
        public ImmutableList<GpuStats> Gpus { get; }
    }
}