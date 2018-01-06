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
            using (var buffer = new MemoryStream())
            {
                await ReceivingStream.CopyToAsync(buffer);

                var contentBytes = buffer.ToArray();
                if (contentBytes.Length == 0)
                    return null;

                var content = Encoding.GetString(contentBytes);

                return content;
            }
        }

        protected override async Task WriteCoreAsync(string content, Encoding contentEncoding, CancellationToken cancellationToken)
        {
            var contentBytes = contentEncoding.GetBytes(TransformContent(content));

            await SendingStream.WriteAsync(contentBytes, 0, contentBytes.Length, cancellationToken);
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