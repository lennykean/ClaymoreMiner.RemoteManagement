using System.IO;
using StreamJsonRpc;

namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal abstract class RpcClientFactory
    {
        public abstract JsonRpc Create(Stream stream);
    }
}