using ClaymoreMiner.RemoteManagement.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ClaymoreMiner.RemoteManagement.Tests
{
    using Mapper;

    [TestFixture]
    public class ModelTests
    {
        [TestCase(TestName = "MinerStatistics is deserialized correctly")]
        public void MinerStatisticsIsDeserializedCorrectly()
        {
            // arrange
            var statsArray = new []
            {
                "9.3 - ETH",
                "21",
                "182724;51;3",
                "30502;30457;30297",
                "0;0;0",
                "off;off;off",
                "53;71;57;67;61;72",
                "eth-eu1.nanopool.org:9999",
                "2;1;0;0"
            };
            var mapper = new MinerStatisticsMapper();
            var stats = mapper.Map(statsArray);

            // act
            var originalJson = JsonConvert.SerializeObject(stats);
            var deserializedStats = JsonConvert.DeserializeObject<MinerStatistics>(originalJson);
            var reSerializedJson = JsonConvert.SerializeObject(deserializedStats);

            // assert
            Assert.That(reSerializedJson, Is.EqualTo(originalJson));
        }
    }
}