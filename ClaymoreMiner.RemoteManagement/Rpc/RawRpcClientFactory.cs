using System.IO;
using System.Text;
using StreamJsonRpc;

namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal class RawRpcClientFactory : RpcClientFactory
    {
        public override JsonRpc Create(Stream stream)
        {
            var rpcClient = new JsonRpc(new RawMessageHandler(stream, Encoding.ASCII));

            rpcClient.StartListening();

            return rpcClient;
        }
    }
}
