using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using StreamJsonRpc;

namespace ClaymoreMiner.RemoteManagement
{
    using Mapper;
    using Models;
    using Rpc;

    public class RemoteManagementClient
    {
        public RemoteManagementClient(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public string Address { get; }
        public int Port { get; }

        public async Task<MinerStatistics> GetStatisticsAsync()
        {
            var stats = await InvokeAsync<string[]>("miner_getstat1");

            return stats.ToMinerStatistics();
        }

        public async Task RestartMinerAsync(string password = null)
        {
            await InvokeAsync("miner_restart");
        }

        public async Task RebootMinerAsync(string password = null)
        {
            await InvokeAsync("miner_reboot");
        }

        public async Task UpdateGpuAsync(GpuMode mode)
        {
            await UpdateGpuAsync(-1, mode);
        }

        public async Task UpdateGpuAsync(int gpuIndex, GpuMode mode)
        {
            await InvokeAsync("control_gpu", gpuIndex, mode);
        }

        private async Task<TResult> InvokeAsync<TResult>(string method, params object[] parameters)
        {
            using (var client = new TcpClient(Address, Port))
            using (var stream = client.GetStream())
            using (var rpcClient = new JsonRpc(new RawMessageHandler(stream, Encoding.ASCII)))
            {
                rpcClient.StartListening();

                return await rpcClient.InvokeAsync<TResult>(method, parameters);
            }
        }

        private async Task InvokeAsync(string method, params object[] parameters)
        {
            using (var client = new TcpClient(Address, Port))
            using (var stream = client.GetStream())
            using (var rpcClient = new JsonRpc(new RawMessageHandler(stream, Encoding.ASCII)))
            {
                rpcClient.StartListening();

                await rpcClient.NotifyAsync(method, parameters);
            }
        }
    }
}