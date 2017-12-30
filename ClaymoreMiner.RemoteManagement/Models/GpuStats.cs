namespace ClaymoreMiner.RemoteManagement.Models
{
    public class GpuStats
    {
        public GpuStats(GpuMode mode, int ethereumHashrate, int decredHashrate, int temperature, int fanSpeed)
        {
            Mode = mode;
            EthereumHashrate = ethereumHashrate;
            DecredHashrate = decredHashrate;
            Temperature = temperature;
            FanSpeed = fanSpeed;
        }
        public GpuMode Mode { get; }
        public int EthereumHashrate { get; }
        public int DecredHashrate { get; }
        public int Temperature { get; }
        public int FanSpeed { get; }
    }
}