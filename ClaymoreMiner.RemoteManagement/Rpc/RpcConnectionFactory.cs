namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal abstract class RpcConnectionFactory
    {
        public abstract RpcConnection Create();
    }
}