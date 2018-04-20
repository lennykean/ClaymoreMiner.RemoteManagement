# ClaymoreMiner.RemoteManagement

.NET client library for the claymore miner remote management API

Also tested and works with Phoenix Miner

[![Build status](https://ci.appveyor.com/api/projects/status/p8kto5lramigius7/branch/master?svg=true)](https://ci.appveyor.com/project/lennykean/claymoreminer-remotemanagement)
[![NuGet](https://img.shields.io/nuget/v/ClaymoreMiner.RemoteManagement.svg)](https://www.nuget.org/packages/ClaymoreMiner.RemoteManagement)

## Installation

### .NET CLI
```shell
> dotnet add package ClaymoreMiner.RemoteManagement
```
### Package Manager
```shell
PM> Install-Package ClaymoreMiner.RemoteManagement
```

## Usage

### Create a client

#### Create a new client using the default port (3333)

```csharp
var client = new RemoteManagementClient(address);
```

#### If the miner is configured with a password for remote management, supply the password in the constructor

```csharp
var client = new RemoteManagementClient(address, port, password);
```

### Calling remote management APIs

#### Get statistics

```csharp
// returns a MinerStatistics object
var stats = await client.GetStatisticsAsync();
```

#### Controlling the miner

```csharp
// Restart the miner
await client.RestartMinerAsync();

// Reboot the miner
await client.RebootMinerAsync();

// Set the mode of all GPUs (Disabled, EthereumOnly, or Dual)
await client.SetGpuModeAsync(mode);

// Set the mode of a single GPU
await client.SetGpuModeAsync(gpuIndex, mode);
```

### Miner Statistics data

#### MinerStatistics

Property|Description|Type
---|---|---
MinerVersion|Miner version|```string```
Uptime|Miner uptime|```TimeSpan```
Ethereum|Ethereum statistics|```PoolStats```
Decred|Decred statistics|```PoolStats```
Gpus|Statistics for all GPUs|```List<GpuStats>```

#### PoolStats

Property|Description|Type
---|---|---
Pool|Mining pool address|```string```
Hashrate|Total hashrate|```int```
Shares|Total shares|```int```
RejectedShares|Number of rejected shares|```int```
InvalidShares|Number of pool switches|```int```
PoolSwitches|Number of invalid shares|```int```

#### GpuStats

Property|Description|Type
---|---|---
Mode|GPU Mode|```GpuMode```
EthereumHashrate|Ethereum hashrate in MH/s|```int```
DecredHashrate|Decred hashrate in MH/s|```int```
Temperature|GPU temperature in celsius|```int```
FanSpeed|Fanspeed %|```int```
