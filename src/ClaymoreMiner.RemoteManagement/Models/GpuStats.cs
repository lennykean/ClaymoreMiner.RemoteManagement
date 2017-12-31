using Newtonsoft.Json;

namespace ClaymoreMiner.RemoteManagement.Models
{
    /// <summary>
    /// GPU statistics
    /// </summary>
    public class GpuStats
    {
        [JsonConstructor]
        public GpuStats(GpuMode mode, int ethereumHashrate, int decredHashrate, int temperature, int fanSpeed)
        {
            Mode = mode;
            EthereumHashrate = ethereumHashrate;
            DecredHashrate = decredHashrate;
            Temperature = temperature;
            FanSpeed = fanSpeed;
        }
        /// <summary>
        /// GPU Mode
        /// </summary>
        public GpuMode Mode { get; }
        /// <summary>
        /// Ethereum hashrate in MH/s
        /// </summary>
        public int EthereumHashrate { get; }
        /// <summary>
        /// Decred hashrate in MH/s
        /// </summary>
        public int DecredHashrate { get; }
        /// <summary>
        /// GPU temperature in celsius
        /// </summary>
        public int Temperature { get; }
        /// <summary>
        /// Fanspeed %
        /// </summary>
        public int FanSpeed { get; }
    }
}