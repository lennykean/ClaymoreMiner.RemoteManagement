using System;
using System.Linq;

using NUnit.Framework;

namespace ClaymoreMiner.RemoteManagement.Tests
{
    using Mapper;

    using Models;

    [TestFixture]
    public class MapperTests
    {
        [TestCase("9.3 - ETH", 21,
            new []
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
            },
            TestName = "Mapper sets miner properties")]
        [TestCase("9.3 - ETH", 21,
            new []
            {
                "9.3 - ETH",
                "21",
                "182724;51;3",
                "30502;30457;30297",
                "58156;81;4",
                "30481;30479;30505",
                "53;71;57;67;61;72",
                "eth-eu1.nanopool.org:9999;dcr.suprnova.cc:3256",
                "2;1;8;7"
            },
            TestName = "Mapper sets miner properties - dual mining")]
        public void MapperSetsMinerProperties(string version, int uptime, string[] statsArray)
        {
            // act
            var stats = statsArray.ToMinerStatistics();

            // assert
            Assert.That(stats, Has.Property(nameof(MinerStatistics.MinerVersion)).EqualTo(version));
            Assert.That(stats, Has.Property(nameof(MinerStatistics.Uptime)).EqualTo(new TimeSpan(0, uptime, 0)));
        }

        [TestCase("eth-eu1.nanopool.org:9999", 182724, 51, 3, 2, 1,
            new []
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
            },
            TestName = "Mapper sets ethereum properties")]
        [TestCase("eth-eu1.nanopool.org:9999", 182724, 51, 3, 2, 1,
            new []
            {
                "9.3 - ETH",
                "21",
                "182724;51;3",
                "30502;30457;30297",
                "58156;81;4",
                "30481;30479;30505",
                "53;71;57;67;61;72",
                "eth-eu1.nanopool.org:9999;dcr.suprnova.cc:3256",
                "2;1;8;7"
            },
            TestName = "Mapper sets ethereum properties - dual mining")]
        public void MapperSetsEthereumProperties(string pool, int hashrate, int shares, int rejectedShares, int invalidShares, int poolSwitches, string[] statsArray)
        {
            // act
            var stats = statsArray.ToMinerStatistics();

            // assert
            Assert.That(stats.Ethereum, Is.Not.Null);
            Assert.That(stats.Ethereum, Has.Property(nameof(PoolStats.Pool)).EqualTo(pool));
            Assert.That(stats.Ethereum, Has.Property(nameof(PoolStats.Hashrate)).EqualTo(hashrate));
            Assert.That(stats.Ethereum, Has.Property(nameof(PoolStats.Shares)).EqualTo(shares));
            Assert.That(stats.Ethereum, Has.Property(nameof(PoolStats.RejectedShares)).EqualTo(rejectedShares));
            Assert.That(stats.Ethereum, Has.Property(nameof(PoolStats.InvalidShares)).EqualTo(invalidShares));
            Assert.That(stats.Ethereum, Has.Property(nameof(PoolStats.PoolSwitches)).EqualTo(poolSwitches));
        }

        [TestCase(null, 0, 0, 0, 0, 0,
            new []
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
            },
            TestName = "Mapper sets decred properties")]
        [TestCase("dcr.suprnova.cc:3256", 58156, 81, 4, 8, 7,
            new []
            {
                "9.3 - ETH",
                "21",
                "182724;51;3",
                "30502;30457;30297",
                "58156;81;4",
                "30481;30479;30505",
                "53;71;57;67;61;72",
                "eth-eu1.nanopool.org:9999;dcr.suprnova.cc:3256",
                "2;1;8;7"
            },
            TestName = "Mapper sets decred properties - dual mining")]
        public void MapperSetsDecredProperties(string pool, int hashrate, int shares, int rejectedShares, int invalidShares, int poolSwitches, string[] statsArray)
        {
            // act
            var stats = statsArray.ToMinerStatistics();

            // assert
            Assert.That(stats.Decred, Is.Not.Null);
            Assert.That(stats.Decred, Has.Property(nameof(PoolStats.Pool)).EqualTo(pool));
            Assert.That(stats.Decred, Has.Property(nameof(PoolStats.Hashrate)).EqualTo(hashrate));
            Assert.That(stats.Decred, Has.Property(nameof(PoolStats.Shares)).EqualTo(shares));
            Assert.That(stats.Decred, Has.Property(nameof(PoolStats.RejectedShares)).EqualTo(rejectedShares));
            Assert.That(stats.Decred, Has.Property(nameof(PoolStats.InvalidShares)).EqualTo(invalidShares));
            Assert.That(stats.Decred, Has.Property(nameof(PoolStats.PoolSwitches)).EqualTo(poolSwitches));
        }

        [TestCase(
            new [] { GpuMode.EthereumOnly, GpuMode.EthereumOnly, GpuMode.EthereumOnly },
            new [] { 30502, 30457, 30297 },
            new [] { 0, 0, 0 },
            new [] { 53, 57, 61 },
            new [] { 71, 67, 72 },
            new []
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
            },
            TestName = "Mapper sets GPU properties")]
        [TestCase(
            new [] { GpuMode.Dual, GpuMode.Dual, GpuMode.Dual },
            new [] { 30502, 30457, 30297 },
            new [] { 30481, 30479, 30505 },
            new [] { 53, 57, 61 },
            new [] { 71, 67, 72 },
            new []
            {
                "9.3 - ETH",
                "21",
                "182724;51;3",
                "30502;30457;30297",
                "58156;81;4",
                "30481;30479;30505",
                "53;71;57;67;61;72",
                "eth-eu1.nanopool.org:9999;dcr.suprnova.cc:3256",
                "2;1;8;7"
            },
            TestName = "Mapper sets GPU properties - dual mining")]
        [TestCase(
            new [] { GpuMode.Disabled, GpuMode.Disabled, GpuMode.Disabled },
            new [] { 0, 0, 0 },
            new [] { 0, 0, 0 },
            new [] { 0, 0, 0 },
            new [] { 0, 0, 0 },
            new []
            {
                "9.3 - ETH",
                "21",
                "0;0;0",
                "off;off;off",
                "0;0;0",
                "off;off;off",
                "0;0;0;0;0;0",
                "eth-eu1.nanopool.org:9999",
                "0;0;0;0"
            },
            TestName = "Mapper sets GPU properties - disabled GPUs")]
        [TestCase(
            new [] { GpuMode.EthereumOnly, GpuMode.EthereumOnly, GpuMode.EthereumOnly },
            new [] { 30502, 30457, 30297 },
            new [] { 0, 0, 0 },
            new [] { 0, 0, 0 },
            new [] { 0, 0, 0 },
            new []
            {
                "9.3 - ETH",
                "21",
                "182724;51;3",
                "30502;30457;30297",
                "0;0;0",
                "off;off;off",
                "",
                "eth-eu1.nanopool.org:9999",
                "2;1;0;0"
            },
            TestName = "Mapper sets GPU properties - missing metrics")]
        public void MapperSetsGpuStats(GpuMode[] modes, int[] ethereumHashrates, int[] decredHashrate, int[] temperature, int[] fanSpeed, string[] statsArray)
        {
            // act
            var stats = statsArray.ToMinerStatistics();

            // assert
            Assert.That(stats.Gpus.Select(g => g.Mode).ToArray(), Is.EqualTo(modes));
            Assert.That(stats.Gpus.Select(g => g.EthereumHashrate).ToArray(), Is.EqualTo(ethereumHashrates));
            Assert.That(stats.Gpus.Select(g => g.DecredHashrate).ToArray(), Is.EqualTo(decredHashrate));
            Assert.That(stats.Gpus.Select(g => g.Temperature).ToArray(), Is.EqualTo(temperature));
            Assert.That(stats.Gpus.Select(g => g.FanSpeed).ToArray(), Is.EqualTo(fanSpeed));
        }
    }
}