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
        private static readonly string[] ResponsePayload =
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

        private RpcClientFactory _rpcClientFactory;
        private IMapper<string[], MinerStatistics> _mapper;
        private Mock<RpcConnectionFactory> _mockRpcConnectionFactory;
        private Mock<RpcConnection> _mockRpcConnection;
        private Stream _localStream;
        private Stream _mockServerStream;

        [SetUp]
        public void Setup()
        {
            _rpcClientFactory = new RawRpcClientFactory();
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
            var address = "test-miner-address";
            var port = 3333;

            // act
            var client = new RemoteManagementClient(address, port);

            //
            Assert.That(client, Has.Property(nameof(client.Address)).EqualTo(address));
            Assert.That(client, Has.Property(nameof(client.Port)).EqualTo(port));
        }

        [Test]
        public async Task TestGetStatisticsAsync()
        {
            // arrange
            var client = new RemoteManagementClient(null, 0, _mockRpcConnectionFactory.Object, _rpcClientFactory, _mapper);
            
            // act
            var resultTask = client.GetStatisticsAsync();
            
            // read the message sent to the server
            var mockServerRecieveBuffer = new MemoryStream();
            var mockServerReadTask = _mockServerStream.CopyToAsync(mockServerRecieveBuffer);
            var recievedMessage = JObject.Parse(Encoding.ASCII.GetString(mockServerRecieveBuffer.ToArray()));
            
            // send the response message back to the client
            var sendMessage = JsonConvert.SerializeObject(new {id = recievedMessage["id"], result = ResponsePayload });
            var mockServerSendBuffer = new MemoryStream(Encoding.ASCII.GetBytes(sendMessage));
            var mockServerWriteTask = mockServerSendBuffer.CopyToAsync(_mockServerStream);

            // end the response
            _mockServerStream.Close();
            await Task.WhenAll(mockServerReadTask, mockServerWriteTask);

            // complete task
            var result = await resultTask;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(recievedMessage["method"].ToString(), Is.EqualTo("miner_getstat1"));
            Assert.That(recievedMessage["params"].ToObject<object[]>, Is.Empty);
        }

        [Test]
        public async Task TestRestartMinerAsync()
        {
            // arrange
            var client = new RemoteManagementClient(null, 0, _mockRpcConnectionFactory.Object, _rpcClientFactory, _mapper);

            // act
            var task = client.RestartMinerAsync();

            // read the message sent to the server
            var mockServerRecieveBuffer = new MemoryStream();
            await _mockServerStream.CopyToAsync(mockServerRecieveBuffer);
            var recievedMessage = JObject.Parse(Encoding.ASCII.GetString(mockServerRecieveBuffer.ToArray()));

            // complete task
            await task;

            // assert
            Assert.That(recievedMessage["method"].ToString(), Is.EqualTo("miner_restart"));
            Assert.That(recievedMessage["params"].ToObject<object[]>, Is.Empty);
        }

        [Test]
        public async Task RebootMinerAsync()
        {
            // arrange
            var client = new RemoteManagementClient(null, 0, _mockRpcConnectionFactory.Object, _rpcClientFactory, _mapper);

            // act
            var task = client.RebootMinerAsync();

            // read the message sent to the server
            var mockServerRecieveBuffer = new MemoryStream();
            await _mockServerStream.CopyToAsync(mockServerRecieveBuffer);
            var recievedMessage = JObject.Parse(Encoding.ASCII.GetString(mockServerRecieveBuffer.ToArray()));

            // complete task
            await task;

            // assert
            Assert.That(recievedMessage["method"].ToString(), Is.EqualTo("miner_reboot"));
            Assert.That(recievedMessage["params"].ToObject<object[]>, Is.Empty);
        }

        [TestCase(1, GpuMode.Disabled)]
        [TestCase(2, GpuMode.Dual)]
        [TestCase(3, GpuMode.EthereumOnly)]
        public async Task SetGpuModeAsync(int index, GpuMode mode)
        {
            // arrange
            var client = new RemoteManagementClient(null, 0, _mockRpcConnectionFactory.Object, _rpcClientFactory, _mapper);

            // act
            var task = client.SetGpuModeAsync(index, mode);

            // read the message sent to the server
            var mockServerRecieveBuffer = new MemoryStream();
            await _mockServerStream.CopyToAsync(mockServerRecieveBuffer);
            var recievedMessage = JObject.Parse(Encoding.ASCII.GetString(mockServerRecieveBuffer.ToArray()));

            // complete task
            await task;

            // assert
            Assert.That(recievedMessage["method"].ToString(), Is.EqualTo("control_gpu"));
            Assert.That(recievedMessage["params"].ToObject<object[]>, Is.EqualTo(new object[] {index, (int)mode}));
        }

        [TestCase(GpuMode.Disabled)]
        [TestCase(GpuMode.Dual)]
        [TestCase(GpuMode.EthereumOnly)]
        public async Task SetGpuModeAsync(GpuMode mode)
        {
            // arrange
            var client = new RemoteManagementClient(null, 0, _mockRpcConnectionFactory.Object, _rpcClientFactory, _mapper);

            // act
            var task = client.SetGpuModeAsync(mode);

            // read the message sent to the server
            var mockServerRecieveBuffer = new MemoryStream();
            await _mockServerStream.CopyToAsync(mockServerRecieveBuffer);
            var recievedMessage = JObject.Parse(Encoding.ASCII.GetString(mockServerRecieveBuffer.ToArray()));

            // complete task
            await task;

            // assert
            Assert.That(recievedMessage["method"].ToString(), Is.EqualTo("control_gpu"));
            Assert.That(recievedMessage["params"].ToObject<object[]>, Is.EqualTo(new object[] { -1, (int)mode }));
        }
    }
}