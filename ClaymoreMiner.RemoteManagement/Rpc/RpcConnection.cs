using System;
using System.IO;

namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal abstract class RpcConnection : IDisposable
    {
        public abstract Stream GetStream();
        public abstract void Dispose();
    }
}
