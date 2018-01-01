using System.Threading.Tasks;

namespace ClaymoreMiner.RemoteManagement
{
    using Models;
    
    /// <summary>
    /// Remote management client for the claymore miner
    /// </summary>
    public interface IRemoteManagementClient
    {
        /// <summary>
        /// Miner Address
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Miner remote management port 
        /// </summary>
        /// <remarks>The default port is 3333</remarks>
        int Port { get; }

        /// <summary>
        /// Gets current statistics
        /// </summary>
        Task<MinerStatistics> GetStatisticsAsync();

        /// <summary>
        /// Restarts miner
        /// </summary>
        Task RestartMinerAsync();

        /// <summary>
        /// Reboots miner
        /// </summary>
        /// <remarks>
        /// Calls "reboot.bat" for Windows, or "reboot.bash" (or "reboot.sh") for Linux.
        /// </remarks>
        Task RebootMinerAsync();

        /// <summary>
        /// Set the mode for all GPUs
        /// </summary>
        /// <param name="mode">GPU mode</param>
        Task SetGpuModeAsync(GpuMode mode);

        /// <summary>
        /// Set the mode for GPU with at specified index
        /// </summary>
        /// <param name="gpuIndex">GPU index</param>
        /// <param name="mode">GPU mode</param>
        Task SetGpuModeAsync(int gpuIndex, GpuMode mode);
    }
}