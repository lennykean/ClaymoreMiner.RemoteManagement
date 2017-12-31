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
    public class RemoteManagementClient
    {
        private readonly RpcConnectionFactory _rpcConnectionFactory;
        private readonly RpcClientFactory _rpcClientFactory;
        private readonly IMapper<string[], MinerStatistics> _mapper;

        public RemoteManagementClient(string address, int port) : this(address, port, null)
        {
        }
        
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

        public string Address { get; }
        public int Port { get; }
        internal string Password { get; }

        public async Task<MinerStatistics> GetStatisticsAsync()
        {
            var stats = await InvokeAsync<string[]>("miner_getstat1");

            return _mapper.Map(stats);
        }

        public async Task RestartMinerAsync()
        {
            await InvokeAsync("miner_restart");
        }

        public async Task RebootMinerAsync()
        {
            await InvokeAsync("miner_reboot");
        }

        public async Task SetGpuModeAsync(GpuMode mode)
        {
            await SetGpuModeAsync(-1, mode);
        }

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