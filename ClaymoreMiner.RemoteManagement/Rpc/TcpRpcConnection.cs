using System.IO;
using System.Net.Sockets;

namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal class TcpRpcConnection : RpcConnection
    {
        private readonly TcpClient _tcpClient;

        public TcpRpcConnection(string address, int port)
        {
            _tcpClient = new TcpClient(address, port);
        }

        public override Stream GetStream()
        {
            return _tcpClient.GetStream();
        }

        public override void Dispose()
        {
            _tcpClient.Close();
        }
    }
}
