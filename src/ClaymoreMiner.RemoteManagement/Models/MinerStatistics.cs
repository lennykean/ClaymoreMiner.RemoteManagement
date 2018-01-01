using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace ClaymoreMiner.RemoteManagement.Models
{
    /// <summary>
    /// Miner statistics
    /// </summary>
    public class MinerStatistics
    {
        /// <summary>
        /// Initializes a new <see cref="MinerStatistics" />
        /// </summary>
        [JsonConstructor]
        public MinerStatistics(string minerVersion, TimeSpan uptime, PoolStats ethereum, PoolStats decred, IEnumerable<GpuStats> gpus)
        {
            MinerVersion = minerVersion;
            Uptime = uptime;
            Ethereum = ethereum;
            Decred = decred;
            Gpus = gpus.ToImmutableList();
        }
        /// <summary>
        /// Miner version
        /// </summary>
        public string MinerVersion { get; }
        /// <summary>
        /// Miner uptime
        /// </summary>
        public TimeSpan Uptime { get; }
        /// <summary>
        /// Ethereum statistics
        /// </summary>
        public PoolStats Ethereum { get; }
        /// <summary>
        /// Decred statistics
        /// </summary>
        public PoolStats Decred { get; }
        /// <summary>
        /// Statistics for all GPUs
        /// </summary>
        public ImmutableList<GpuStats> Gpus { get; }
    }
}