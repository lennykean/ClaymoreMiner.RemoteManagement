using System;
using System.Collections.Generic;
using System.Linq;

namespace ClaymoreMiner.RemoteManagement.Mapper
{
    using Models;

    internal class MinerStatisticsMapper : IMapper<string[], MinerStatistics>
    {
        public MinerStatistics Map(string[] source)
        {
            var (version, _) = source.TryGet(0);
            var (uptimeMinutes, _) = source.TryGetAndParse(1);
            var uptime = TimeSpan.FromMinutes(uptimeMinutes);

            var ethereumStats = source.TryGet(2).value?.Split(';');
            var ethereumGpuHashrates = source.TryGet(3).value?.Split(';');
            var decredStats = source.TryGet(4).value?.Split(';');
            var decredGpuHashrates = source.TryGet(5).value?.Split(';');
            var allGpuMetrics = source.TryGet(6).value?.Split(';')
                .Select((value, index) => new { value, index })
                .GroupBy(s => s.index / 2)
                .Select(g => g.Select(s => s.value).ToArray())
                .ToArray();
            var pools = source.TryGet(7).value?.Split(';');
            var additionalData = source.TryGet(8).value?.Split(';')
                .Select((value, index) => new { value, index })
                .GroupBy(s => s.index / 2)
                .Select(g => g.Select(s => s.value).ToArray())
                .ToArray();

            var ethereum = GetEthereumPoolStats(pools, ethereumStats, additionalData);
            var decred = GetDecredPoolStats(pools, decredStats, additionalData);
            var gpus = GetAllGpuStats(ethereumGpuHashrates, decredGpuHashrates, allGpuMetrics);

            var status = new MinerStatistics(
                version,
                uptime,
                ethereum,
                decred,
                gpus);

            return status;
        }

        private static PoolStats GetEthereumPoolStats(string[] pools, string[] ethereumStats, string[][] additionalData)
        {
            var (ethereumPool, _) = pools.TryGet(0);
            var (additionalEthereumData, _) = additionalData.TryGet(0);

            return GetPoolStats(ethereumPool, ethereumStats, additionalEthereumData);
        }

        private static PoolStats GetDecredPoolStats(string[] pools, string[] decredStats, string[][] additionalData)
        {
            var (decredPool, _) = pools.TryGet(1);
            var (additionalDecredData, _) = additionalData.TryGet(1);

            return GetPoolStats(decredPool, decredStats, additionalDecredData);
        }

        private static PoolStats GetPoolStats(string pool, string[] stats, string[] additionalData)
        {
            var (hashRate, _) = stats.TryGetAndParse(0);
            var (shares, _) = stats.TryGetAndParse(1);
            var (rejectedShares, _) = stats.TryGetAndParse(2);

            var (invalidShares, _) = additionalData.TryGetAndParse(0);
            var (poolSwitches, _) = additionalData.TryGetAndParse(1);

            var poolStats = new PoolStats(
                pool,
                hashRate,
                shares,
                rejectedShares,
                invalidShares,
                poolSwitches);

            return poolStats;
        }

        private static IEnumerable<GpuStats> GetAllGpuStats(string[] ethereumGpuHashrates, string[] decredGpuHashrates, string[][] allGpuMetrics)
        {
            var gpuCount = Math.Max(Math.Max(ethereumGpuHashrates.Length, decredGpuHashrates.Length), allGpuMetrics.Length);

            for (var i = 0; i < gpuCount; i++)
            {
                var parseEthereumHashrateResult = ethereumGpuHashrates.TryGetAndParse(i);
                var parseDecredHashrateResult = decredGpuHashrates.TryGetAndParse(i);
                var (metrics, _) = allGpuMetrics.TryGet(i);

                yield return GetGpuStats(parseEthereumHashrateResult, parseDecredHashrateResult, metrics);
            }
        }

        private static GpuStats GetGpuStats(
            (int value, bool success) parseEthereumHashrateResult, 
            (int value, bool success) parseDecredHashrateResult, 
            string[] metrics)
        {
            var mode = GpuMode.Disabled;

            if (parseEthereumHashrateResult.success)
                mode = GpuMode.EthereumOnly;
            if (parseDecredHashrateResult.success)
                mode = GpuMode.Dual;

            var (temperature, _) = metrics.TryGetAndParse(0);
            var (fanSpeed, _) = metrics.TryGetAndParse(1);

            var gpuStats = new GpuStats(
                mode,
                parseEthereumHashrateResult.value,
                parseDecredHashrateResult.value,
                temperature,
                fanSpeed);

            return gpuStats;
        }

        private static GpuStats GetGpuStats(string ethereumHashrateString, string decredHashrateString, string[] metrics)
        {
            var mode = GpuMode.Disabled;

            if (int.TryParse(ethereumHashrateString, out var ethereumHashrate))
                mode = GpuMode.EthereumOnly;
            if (int.TryParse(decredHashrateString, out var decredHashrate))
                mode = GpuMode.Dual;

            var (temperature, _) = metrics.TryGetAndParse(0);
            var (fanSpeed, _) = metrics.TryGetAndParse(1);

            var gpuStats = new GpuStats(
                mode,
                ethereumHashrate,
                decredHashrate,
                temperature,
                fanSpeed);

            return gpuStats;
        }
    }
}