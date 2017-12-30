
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using StreamJsonRpc;

namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal class RawMessageHandler : DelimitedMessageHandler
    {
        public RawMessageHandler(Stream stream, Encoding encoding) : base(stream, stream, encoding)
        {
        }

        protected override async Task<string> ReadCoreAsync(CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream())
            {
                await ReceivingStream.CopyToAsync(memoryStream);

                var buffer = memoryStream.ToArray();
                if (buffer.Length == 0)
                    return null;

                return Encoding.GetString(buffer);
            }
        }

        protected override async Task WriteCoreAsync(string content, Encoding contentEncoding, CancellationToken cancellationToken)
        {
            var buffer = contentEncoding.GetBytes(content);

            await SendingStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        }
    }
}