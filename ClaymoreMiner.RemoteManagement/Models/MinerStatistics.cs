using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace ClaymoreMiner.RemoteManagement.Models
{
    public class MinerStatistics
    {
        [JsonConstructor]
        public MinerStatistics(string minerVersion, TimeSpan uptime, PoolStats ethereum, PoolStats decred, IEnumerable<GpuStats> gpus)
        {
            MinerVersion = minerVersion;
            Uptime = uptime;
            Ethereum = ethereum;
            Decred = decred;
            Gpus = gpus.ToImmutableList();
        }
        public string MinerVersion { get; }
        public TimeSpan Uptime { get; }
        public PoolStats Ethereum { get; }
        public PoolStats Decred { get; }
        public ImmutableList<GpuStats> Gpus { get; }
    }
}