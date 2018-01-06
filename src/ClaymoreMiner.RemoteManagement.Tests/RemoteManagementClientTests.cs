using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ClaymoreMiner.RemoteManagement.Tests
{
    using Mapper;
    using Models;
    using Rpc;

    [TestFixture]
    public class RemoteManagementClientTests
    {
        private static readonly string[] GetStatisticsResponsePayload =
        {
            "9.3 - ETH",
            "21",
            "182724;51;0",
            "30502;30457;30297;30481;30479;30505",
            "0;0;0",
            "off;off;off;off;off;off",
            "53;71;57;67;61;72;55;70;59;71;61;70",
            "eth-eu1.nanopool.org:9999",
            "0;0;0;0"
        };
        
        private IMapper<string[], MinerStatistics> _mapper;
        private Mock<RpcConnectionFactory> _mockRpcConnectionFactory;
        private Mock<RpcConnection> _mockRpcConnection;
        private Stream _localStream;
        private Stream _mockServerStream;

        [SetUp]
        public void Setup()
        {
            _mapper = new MinerStatisticsMapper();

            _mockRpcConnectionFactory = new Mock<RpcConnectionFactory>();
            _mockRpcConnection = new Mock<RpcConnection>();
            (_localStream, _mockServerStream) = Nerdbank.FullDuplexStream.CreateStreams();

            _mockRpcConnectionFactory.Setup(m => m.Create()).Returns(() => _mockRpcConnection.Object);
            _mockRpcConnection.Setup(m => m.GetStream()).Returns(() => _localStream);
        }
        
        [TestCase(TestName = "RemoteManagementClient.ctor sets properties")]
        public void ConstructorSetsProperties()
        {
            // arrange
            const string address = "test-miner-address";
            const int port = 3333;
            const string password = "test-password";

            // act
            var client = new RemoteManagementClient(address, port, password);

            // assert
            Assert.That(client, Has.Property(nameof(client.Address)).EqualTo(address));
            Assert.That(client, Has.Property(nameof(client.Port)).EqualTo(port));
            Assert.That(client, Has.Property(nameof(client.Password)).EqualTo(password));
        }

        [TestCase(null, TestName = "RemoteManagementClient.GetStatisticsAsync")]
        [TestCase("test-password", TestName = "RemoteManagementClient.GetStatisticsAsync with password")]
        public async Task GetStatisticsAsync(string password)
        {
            // arrange
            var rpcClientFactory = new ClaymoreApiRpcClientFactory(password);
            var client = new RemoteManagementClient(null, 0, null, _mockRpcConnectionFactory.Object, rpcClientFactory, _mapper);
            
            // act
            var resultTask = client.GetStatisticsAsync();
            var requestObject = await RespondToJsonRpcRequest(_mockServerStream, GetStatisticsResponsePayload);

            // complete task
            var result = await resultTask;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(requestObject["method"].ToString(), Is.EqualTo("miner_getstat1"));
            Assert.That(requestObject["params"].ToObject<object[]>, Is.Empty);
            Assert.That(requestObject["pwd"]?.ToString(), Is.EqualTo(password));
        }
        
        [TestCase(null, TestName = "RemoteManagementClient.RestartMinerAsync")]
        [TestCase("test-password", TestName = "RemoteManagementClient.RestartMinerAsync with password")]
        public async Task RestartMinerAsync(string password)
        {
            // arrange
            var rpcClientFactory = new ClaymoreApiRpcClientFactory(password);
            var client = new RemoteManagementClient(null, 0, null, _mockRpcConnectionFactory.Object, rpcClientFactory, _mapper);

            // act
            var task = client.RestartMinerAsync();
            var requestObject = await RespondToJsonRpcRequest(_mockServerStream, null);

            // complete task
            await task;

            // assert
            Assert.That(requestObject["method"].ToString(), Is.EqualTo("miner_restart"));
            Assert.That(requestObject["params"].ToObject<object[]>, Is.Empty);
            Assert.That(requestObject["pwd"]?.ToString(), Is.EqualTo(password));
        }

        [TestCase(null, TestName = "RemoteManagementClient.RebootMinerAsync")]
        [TestCase("test-password", TestName = "RemoteManagementClient.RebootMinerAsync with password")]
        public async Task RebootMinerAsync(string password)
        {
            // arrange
            var rpcClientFactory = new ClaymoreApiRpcClientFactory(password);
            var client = new RemoteManagementClient(null, 0, null, _mockRpcConnectionFactory.Object, rpcClientFactory, _mapper);

            // act
            var task = client.RebootMinerAsync();
            var requestObject = await RespondToJsonRpcRequest(_mockServerStream, null);

            // complete task
            await task;

            // assert
            Assert.That(requestObject["method"].ToString(), Is.EqualTo("miner_reboot"));
            Assert.That(requestObject["params"].ToObject<object[]>, Is.Empty);
            Assert.That(requestObject["pwd"]?.ToString(), Is.EqualTo(password));
        }

        [TestCase(null, 1, GpuMode.Disabled, TestName = "RemoteManagementClient.SetGpuModeAsync(int, Disabled)")]
        [TestCase(null, 3, GpuMode.EthereumOnly, TestName = "RemoteManagementClient.SetGpuModeAsync(int, EthereumOnly)")]
        [TestCase(null, 2, GpuMode.Dual, TestName = "RemoteManagementClient.SetGpuModeAsync(int, Dual)")]
        [TestCase("test-password", 1, GpuMode.Disabled, TestName = "RemoteManagementClient.SetGpuModeAsync(int, Disabled) with password")]
        [TestCase("test-password", 3, GpuMode.EthereumOnly, TestName = "RemoteManagementClient.SetGpuModeAsync(int, EthereumOnly) with password")]
        [TestCase("test-password", 2, GpuMode.Dual, TestName = "RemoteManagementClient.SetGpuModeAsync(int, Dual) with password")]
        public async Task SetGpuModeAsync(string password, int index, GpuMode mode)
        {
            // arrange
            var rpcClientFactory = new ClaymoreApiRpcClientFactory(password);
            var client = new RemoteManagementClient(null, 0, null, _mockRpcConnectionFactory.Object, rpcClientFactory, _mapper);

            // act
            var task = client.SetGpuModeAsync(index, mode);
            var requestObject = await RespondToJsonRpcRequest(_mockServerStream, null);

            // complete task
            await task;

            // assert
            Assert.That(requestObject["method"].ToString(), Is.EqualTo("control_gpu"));
            Assert.That(requestObject["params"].ToObject<object[]>, Is.EqualTo(new[] {index.ToString(), ((int)mode).ToString() }));
            Assert.That(requestObject["pwd"]?.ToString(), Is.EqualTo(password));
        }
        
        [TestCase(null, GpuMode.Disabled, TestName = "RemoteManagementClient.SetGpuModeAsync(Disabled)")]
        [TestCase(null, GpuMode.EthereumOnly, TestName = "RemoteManagementClient.SetGpuModeAsync(EthereumOnly)")]
        [TestCase(null, GpuMode.Dual, TestName = "RemoteManagementClient.SetGpuModeAsync(Dual)")]
        [TestCase("test-password", GpuMode.Disabled, TestName = "RemoteManagementClient.SetGpuModeAsync(Disabled) with password")]
        [TestCase("test-password", GpuMode.EthereumOnly, TestName = "RemoteManagementClient.SetGpuModeAsync(EthereumOnly) with password")]
        [TestCase("test-password", GpuMode.Dual, TestName = "RemoteManagementClient.SetGpuModeAsync(Dual) with password")]
        public async Task SetGpuModeAsync(string password, GpuMode mode)
        {
            // arrange
            var rpcClientFactory = new ClaymoreApiRpcClientFactory(password);
            var client = new RemoteManagementClient(null, 0, null, _mockRpcConnectionFactory.Object, rpcClientFactory, _mapper);

            // act
            var task = client.SetGpuModeAsync(mode);
            var requestObject = await RespondToJsonRpcRequest(_mockServerStream, null);

            // complete task
            await task;

            // assert
            Assert.That(requestObject["method"].ToString(), Is.EqualTo("control_gpu"));
            Assert.That(requestObject["params"].ToObject<object[]>, Is.EqualTo(new[] { "-1", ((int)mode).ToString() }));
            Assert.That(requestObject["pwd"]?.ToString(), Is.EqualTo(password));
        }

        private static async Task<JObject> RespondToJsonRpcRequest(Stream serverStream, object response)
        {
            // read the message sent to the server
            var requestBuffer = new MemoryStream();
            var readRequestTask = serverStream.CopyToAsync(requestBuffer);
            var requestObject = JObject.Parse(Encoding.ASCII.GetString(requestBuffer.ToArray()));

            // send the response message back to the client
            var responseObject = new { id = requestObject["id"], result = response };
            var responseBuffer = new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(responseObject)));
            var writeResponseTask = responseBuffer.CopyToAsync(serverStream);

            // end the response
            serverStream.Close();
            await Task.WhenAll(readRequestTask, writeResponseTask);

            return requestObject;
        }
    }
}