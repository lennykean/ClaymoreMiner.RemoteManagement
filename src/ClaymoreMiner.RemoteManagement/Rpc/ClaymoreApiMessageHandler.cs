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
            using (var data = new MemoryStream())
            {
                var buffer = new byte[32768];

                while (true)
                {
                    var read = await ReceivingStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    data.Write(buffer, 0, read);

                    var contentBytes = data.ToArray();
                    if (contentBytes.Length == 0)
                        return null;

                    var content = Encoding.GetString(contentBytes);

                    try
                    {
                        JObject.Parse(content);
                        return content;
                    }
                    catch (JsonException)
                    {
                    }
                }
            }
        }

        protected override async Task WriteCoreAsync(string content, Encoding contentEncoding, CancellationToken cancellationToken)
        {
            var contentBytes = contentEncoding.GetBytes(TransformContent(content));

            await SendingStream.WriteAsync(contentBytes, 0, contentBytes.Length, cancellationToken);
        }

        private string TransformContent(string content)
        {
            var contentJson = JObject.Parse(content);

            if (_password != null)
                contentJson.Add("pwd", _password);

            var transformedContent = contentJson.ToString(Formatting.None) + '\n';

            return transformedContent;
        }
    }
}