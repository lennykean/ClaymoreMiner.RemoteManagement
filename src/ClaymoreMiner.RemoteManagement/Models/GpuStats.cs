using Newtonsoft.Json;

namespace ClaymoreMiner.RemoteManagement.Models
{
    /// <summary>
    /// GPU statistics
    /// </summary>
    public class GpuStats
    {
        /// <summary>
        /// GPU Mode
        /// </summary>
        public GpuMode Mode { get; set; }
        /// <summary>
        /// Ethereum hashrate in MH/s
        /// </summary>
        public int EthereumHashrate { get; set; }
        /// <summary>
        /// Decred hashrate in MH/s
        /// </summary>
        public int DecredHashrate { get; set; }
        /// <summary>
        /// GPU temperature in celsius
        /// </summary>
        public int Temperature { get; set; }
        /// <summary>
        /// Fanspeed %
        /// </summary>
        public int FanSpeed { get; set; }
    }
}