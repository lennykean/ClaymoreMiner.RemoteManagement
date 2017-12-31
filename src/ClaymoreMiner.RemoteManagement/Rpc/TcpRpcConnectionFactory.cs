namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal class TcpRpcConnectionFactory : RpcConnectionFactory
    {
        private readonly string _address;
        private readonly int _port;

        public TcpRpcConnectionFactory(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public override RpcConnection Create()
        {
            return new TcpRpcConnection(_address, _port);
        }
    }
}