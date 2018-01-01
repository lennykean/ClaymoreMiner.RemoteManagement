using System.Linq;
using System.Threading.Tasks;

namespace ClaymoreMiner.RemoteManagement
{
    using Mapper; 
    using Models;
    using Rpc;
    
    /// <summary>
    /// Remote management client for the claymore miner
    /// </summary>
    public class RemoteManagementClient : IRemoteManagementClient
    {
        private readonly RpcConnectionFactory _rpcConnectionFactory;
        private readonly RpcClientFactory _rpcClientFactory;
        private readonly IMapper<string[], MinerStatistics> _mapper;
        
        /// <summary>
        /// Initializes a new <see cref="RemoteManagementClient" /> with claymores default remote management port.
        /// </summary>
        /// <param name="address">Miner address</param>
        public RemoteManagementClient(string address) : this(address, 3333, null)
        {
        }
        
        /// <summary>
        /// Initializes a new <see cref="RemoteManagementClient" />.
        /// </summary>
        /// <param name="address">Miner address</param>
        /// <param name="port">Miner remote management port</param>
        public RemoteManagementClient(string address, int port) : this(address, port, null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="RemoteManagementClient" /> with a password.
        /// </summary>
        /// <param name="address">Miner address</param>
        /// <param name="port">Miner remote management port</param>
        /// <param name="password">Miner remote management password</param>
        public RemoteManagementClient(string address, int port, string password) : this(address, port, password, 
            new TcpRpcConnectionFactory(address, port), 
            new ClaymoreApiRpcClientFactory(password), 
            new MinerStatisticsMapper())
        {
        }

        internal RemoteManagementClient(string address, int port, string password,
            RpcConnectionFactory rpcConnectionFactory,
            RpcClientFactory rpcClientFactory,
            IMapper<string[], MinerStatistics> mapper)
        {
            Address = address;
            Port = port;
            Password = password;

            _rpcConnectionFactory = rpcConnectionFactory;
            _rpcClientFactory = rpcClientFactory;
            _mapper = mapper;
        }

        /// <summary>
        /// Miner Address
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Miner remote management port 
        /// </summary>
        /// <remarks>The default port is 3333</remarks>
        public int Port { get; }

        internal string Password { get; }

        /// <summary>
        /// Gets current statistics
        /// </summary>
        public async Task<MinerStatistics> GetStatisticsAsync()
        {
            var stats = await InvokeAsync<string[]>("miner_getstat1");

            return _mapper.Map(stats);
        }

        /// <summary>
        /// Restarts miner
        /// </summary>
        public async Task RestartMinerAsync()
        {
            await InvokeAsync("miner_restart");
        }
        
        /// <summary>
        /// Reboots miner
        /// </summary>
        /// <remarks>
        /// Calls "reboot.bat" for Windows, or "reboot.bash" (or "reboot.sh") for Linux.
        /// </remarks>
        public async Task RebootMinerAsync()
        {
            await InvokeAsync("miner_reboot");
        }

        /// <summary>
        /// Set the mode for all GPUs
        /// </summary>
        /// <param name="mode">GPU mode</param>
        public async Task SetGpuModeAsync(GpuMode mode)
        {
            await SetGpuModeAsync(-1, mode);
        }

        /// <summary>
        /// Set the mode for GPU with at specified index
        /// </summary>
        /// <param name="gpuIndex">GPU index</param>
        /// <param name="mode">GPU mode</param>
        public async Task SetGpuModeAsync(int gpuIndex, GpuMode mode)
        {
            await InvokeAsync("control_gpu", gpuIndex.ToString(), ((int)mode).ToString());
        }

        private async Task<TResult> InvokeAsync<TResult>(string method, params string[] parameters)
        {
            using (var rpcConnection = _rpcConnectionFactory.Create())
            using (var rpcStream = rpcConnection.GetStream())
            using (var rpcClient = _rpcClientFactory.Create(rpcStream))
            {
                return await rpcClient.InvokeAsync<TResult>(method, parameters.Cast<object>().ToArray());
            }
        }

        private async Task InvokeAsync(string method, params string[] parameters)
        {
            using (var rpcConnection = _rpcConnectionFactory.Create())
            using (var rpcStream = rpcConnection.GetStream())
            using (var rpcClient = _rpcClientFactory.Create(rpcStream))
            {
                await rpcClient.NotifyAsync(method, parameters.Cast<object>().ToArray());
            }
        }
    }
}