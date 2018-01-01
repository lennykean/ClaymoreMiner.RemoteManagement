using System.IO;
using StreamJsonRpc;

namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal class ClaymoreApiRpcClientFactory : RpcClientFactory
    {
        private readonly string _password;

        public ClaymoreApiRpcClientFactory(string password)
        {
            _password = password;
        }

        public override JsonRpc Create(Stream stream)
        {
            var rpcClient = new JsonRpc(new ClaymoreApiMessageHandler(_password, stream));

            rpcClient.StartListening();

            return rpcClient;
        }
    }
}
