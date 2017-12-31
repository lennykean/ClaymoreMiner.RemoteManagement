using System.Threading.Tasks;

namespace ClaymoreMiner.RemoteManagement
{
    using Mapper;
    using Models;
    using Rpc;

    public class RemoteManagementClient
    {
        private readonly RpcConnectionFactory _rpcConnectionFactory;
        private readonly RpcClientFactory _rpcClientFactory;
        private readonly IMapper<string[], MinerStatistics> _mapper;

        public RemoteManagementClient(string address, int port) :
            this(address, port, new TcpRpcConnectionFactory(address, port), new RawRpcClientFactory(), new MinerStatisticsMapper())
        {
        }

        internal RemoteManagementClient(string address, int port,
            RpcConnectionFactory rpcConnectionFactory,
            RpcClientFactory rpcClientFactory,
            IMapper<string[], MinerStatistics> mapper)
        {
            Address = address;
            Port = port;

            _rpcConnectionFactory = rpcConnectionFactory;
            _rpcClientFactory = rpcClientFactory;
            _mapper = mapper;
        }

        public string Address { get; }
        public int Port { get; }

        public async Task<MinerStatistics> GetStatisticsAsync()
        {
            var stats = await InvokeAsync<string[]>("miner_getstat1");

            return _mapper.Map(stats);
        }

        public async Task RestartMinerAsync(string password = null)
        {
            await InvokeAsync("miner_restart");
        }

        public async Task RebootMinerAsync(string password = null)
        {
            await InvokeAsync("miner_reboot");
        }

        public async Task SetGpuModeAsync(GpuMode mode)
        {
            await SetGpuModeAsync(-1, mode);
        }

        public async Task SetGpuModeAsync(int gpuIndex, GpuMode mode)
        {
            await InvokeAsync("control_gpu", gpuIndex, mode);
        }

        private async Task<TResult> InvokeAsync<TResult>(string method, params object[] parameters)
        {
            using (var rpcConnection = _rpcConnectionFactory.Create())
            using (var rpcStream = rpcConnection.GetStream())
            using (var rpcClient = _rpcClientFactory.Create(rpcStream))
            {
                return await rpcClient.InvokeAsync<TResult>(method, parameters);
            }
        }

        private async Task InvokeAsync(string method, params object[] parameters)
        {
            using (var rpcConnection = _rpcConnectionFactory.Create())
            using (var rpcStream = rpcConnection.GetStream())
            using (var rpcClient = _rpcClientFactory.Create(rpcStream))
            {
                await rpcClient.NotifyAsync(method, parameters);
            }
        }
    }
}