using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace ClaymoreMiner.RemoteManagement.Rpc
{
    internal class ClaymoreApiMessageHandler : DelimitedMessageHandler
    {
        private readonly string _password;

        public ClaymoreApiMessageHandler(string password, Stream stream) : base(stream, stream, Encoding.ASCII)
        {
            _password = password;
        }

        protected override async Task<string> ReadCoreAsync(CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream())
            {
                await ReceivingStream.CopyToAsync(memoryStream);

                var buffer = memoryStream.ToArray();
                if (buffer.Length == 0)
                    return null;

                var content = Encoding.GetString(buffer);

                return content;
            }
        }

        protected override async Task WriteCoreAsync(string content, Encoding contentEncoding, CancellationToken cancellationToken)
        {
            var buffer = contentEncoding.GetBytes(TransformContent(content));

            await SendingStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        }

        private string TransformContent(string content)
        {
            if (_password == null)
                return content;

            var contentJson = JObject.Parse(content);

            contentJson.Add("pwd", _password);

            return contentJson.ToString(Formatting.None);
        }
    }
}