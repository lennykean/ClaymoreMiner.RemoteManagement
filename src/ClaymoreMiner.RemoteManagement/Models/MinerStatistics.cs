using System;
using System.Collections.Generic;

namespace ClaymoreMiner.RemoteManagement.Models
{
    /// <summary>
    /// Miner statistics
    /// </summary>
    public class MinerStatistics
    {
        /// <summary>
        /// Miner version
        /// </summary>
        public string MinerVersion { get; set; }
        /// <summary>
        /// Miner uptime
        /// </summary>
        public TimeSpan Uptime { get; set; }
        /// <summary>
        /// Ethereum statistics
        /// </summary>
        public PoolStats Ethereum { get; set; }
        /// <summary>
        /// Decred statistics
        /// </summary>
        public PoolStats Decred { get; set; }
        /// <summary>
        /// Statistics for all GPUs
        /// </summary>
        public IList<GpuStats> Gpus { get; set; }
    }
}