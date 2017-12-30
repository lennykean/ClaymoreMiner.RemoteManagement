using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ClaymoreMiner.RemoteManagement.Mapper
{
    using Models;

    internal static class MinerStatisticsMapper
    {
        public static MinerStatistics ToMinerStatistics(this string[] statsArray)
        {
            if (statsArray.Length < 9)
                throw new ArgumentException("Unable to parse miner statistics", nameof(statsArray));

            var version = statsArray[0];
            var uptime = ParseUptime(statsArray[1]);
            var ethereumStats = statsArray[2].Split(';');
            var ethereumGpuHashrates = statsArray[3].Split(';');
            var decredStats = statsArray[4].Split(';');
            var decredGpuHashrates = statsArray[5].Split(';');
            var allGpuMetrics = statsArray[6].Split(';')
                .Select((value, index) => new { value, index })
                .GroupBy(s => s.index / 2)
                .Select(g => g.Select(s => s.value).ToArray())
                .ToArray();
            var pools = statsArray[7].Split(';');
            var additionalData = statsArray[8].Split(';')
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
                gpus.ToImmutableList());

            return status;
        }

        private static TimeSpan ParseUptime(string uptime)
        {
            if (!int.TryParse(uptime, out var uptimeMinutes))
                return TimeSpan.Zero;

            return TimeSpan.FromMinutes(uptimeMinutes);

        }

        private static PoolStats GetEthereumPoolStats(string[] pools, string[] ethereumStats, string[][] additionalData)
        {
            var ethereumPool = pools.Length > 0 ? pools[0] : null;
            var additionalEthereumData = additionalData.Length > 1 ? additionalData[1] : Array.Empty<string>();

            return GetPoolStats(ethereumPool, ethereumStats, additionalEthereumData);
        }

        private static PoolStats GetDecredPoolStats(string[] pools, string[] decredStats, string[][] additionalData)
        {
            var decredPool = pools.Length > 1 ? pools[1] : null;
            var additionalDecredData = additionalData.Length > 1 ? additionalData[1] : Array.Empty<string>();

            return GetPoolStats(decredPool, decredStats, additionalDecredData);
        }

        private static PoolStats GetPoolStats(string pool, string[] stats, string[] additionalData)
        {
            int.TryParse(stats.Length > 0 ? stats[0] : string.Empty, out var hashRate);
            int.TryParse(stats.Length > 1 ? stats[1] : string.Empty, out var shares);
            int.TryParse(stats.Length > 2 ? stats[2] : string.Empty, out var rejectedShares);
            int.TryParse(additionalData.Length > 1 ? additionalData[0] : string.Empty, out var invalidShares);
            int.TryParse(additionalData.Length > 2 ? additionalData[1] : string.Empty, out var poolSwitches);

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
                var ethereumHashrate = ethereumGpuHashrates.Length > i ? ethereumGpuHashrates[i] : string.Empty;
                var decredHashrate = decredGpuHashrates.Length > i ? decredGpuHashrates[i] : string.Empty;
                var metrics = allGpuMetrics.Length > 1 ? allGpuMetrics[i] : Array.Empty<string>();

                yield return GetGpuStats(ethereumHashrate, decredHashrate, metrics);
            }
        }

        private static GpuStats GetGpuStats(string ethereumHashrateString, string decredHashrateString, string[] metrics)
        {
            var mode = GpuMode.Disabled;

            if (int.TryParse(ethereumHashrateString, out var ethereumHashrate))
                mode = GpuMode.EthereumOnly;
            if (int.TryParse(decredHashrateString, out var decredHashrate))
                mode = GpuMode.Dual;

            int.TryParse(metrics.Length > 0 ? metrics[0] : string.Empty, out var temperature);
            int.TryParse(metrics.Length > 1 ? metrics[1] : string.Empty, out var fanSpeed);

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