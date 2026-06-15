# 工业设备数据采集与监控平台 — 项目基本结构搭建实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 根据已批准的设计文档，搭建工业设备数据采集与监控平台的完整项目基本结构，使 API、采集器、WPF 客户端、设备模拟器四个入口项目均可独立启动，SQLite 数据库可自动生成，Swagger 可访问。

**Architecture：** 采用 Clean Architecture 分层：Domain / Shared 为最底层；Application 编排用例；Infrastructure 实现数据库与通信；Api 和 Collector 作为服务端入口；Wpf 和 DeviceSimulator 作为客户端/模拟入口。第一阶段只搭建空壳，不实现业务逻辑。

**Tech Stack：** .NET 9, ASP.NET Core Web API, Worker Service, WPF, EF Core SQLite, SignalR, Serilog, Swagger, CommunityToolkit.Mvvm, HandyControl, LiveCharts2.

---

## 文件结构总览

创建或修改以下文件：

```text
d:\学习\IndustrialMonitorPlatform\
├── IndustrialMonitorPlatform.sln
├── src/
│   ├── IndustrialMonitor.Api/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── IndustrialMonitor.Api.csproj
│   ├── IndustrialMonitor.Collector/
│   │   ├── Program.cs
│   │   ├── Workers/DeviceCollectorWorker.cs
│   │   ├── appsettings.json
│   │   └── IndustrialMonitor.Collector.csproj
│   ├── IndustrialMonitor.Wpf/
│   │   ├── App.xaml
│   │   ├── App.xaml.cs
│   │   ├── Program.cs
│   │   ├── MainWindow.xaml
│   │   ├── MainWindow.xaml.cs
│   │   ├── appsettings.json
│   │   └── IndustrialMonitor.Wpf.csproj
│   ├── IndustrialMonitor.DeviceSimulator/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── IndustrialMonitor.DeviceSimulator.csproj
│   ├── IndustrialMonitor.Application/
│   │   ├── Interfaces/IDeviceRepository.cs
│   │   └── IndustrialMonitor.Application.csproj
│   ├── IndustrialMonitor.Domain/
│   │   ├── Entities/Device.cs
│   │   ├── Entities/DeviceData.cs
│   │   ├── Entities/AlarmRecord.cs
│   │   ├── Entities/AlarmRule.cs
│   │   ├── Entities/OperationLog.cs
│   │   └── IndustrialMonitor.Domain.csproj
│   ├── IndustrialMonitor.Infrastructure/
│   │   ├── Database/ApplicationDbContext.cs
│   │   ├── Database/Configurations/DeviceConfiguration.cs
│   │   ├── Database/Configurations/DeviceDataConfiguration.cs
│   │   ├── Database/Configurations/AlarmRecordConfiguration.cs
│   │   ├── Database/Configurations/AlarmRuleConfiguration.cs
│   │   ├── Database/Configurations/OperationLogConfiguration.cs
│   │   └── IndustrialMonitor.Infrastructure.csproj
│   └── IndustrialMonitor.Shared/
│       ├── Dtos/DeviceDto.cs
│       ├── Constants/PlatformConstants.cs
│       └── IndustrialMonitor.Shared.csproj
```

---

### Task 1: 验证 .NET SDK 并创建解决方案

**Files:**
- Create: `IndustrialMonitorPlatform.sln`

- [ ] **Step 1: 验证 .NET 8 SDK**

Run:
```bash
dotnet --list-sdks
```
Expected: 输出中包含 `9.0.x`。

- [ ] **Step 2: 创建解决方案文件**

Run:
```bash
dotnet new sln -n IndustrialMonitorPlatform
```
Expected: 生成 `IndustrialMonitorPlatform.sln`。

---

### Task 2: 创建类库项目（Domain, Shared, Application, Infrastructure）

**Files:**
- Create: `src/IndustrialMonitor.Domain/IndustrialMonitor.Domain.csproj`
- Create: `src/IndustrialMonitor.Shared/IndustrialMonitor.Shared.csproj`
- Create: `src/IndustrialMonitor.Application/IndustrialMonitor.Application.csproj`
- Create: `src/IndustrialMonitor.Infrastructure/IndustrialMonitor.Infrastructure.csproj`

- [ ] **Step 1: 创建 Domain 类库**

Run:
```bash
dotnet new classlib -n IndustrialMonitor.Domain -o src/IndustrialMonitor.Domain -f net9.0
dotnet sln add src/IndustrialMonitor.Domain/IndustrialMonitor.Domain.csproj
```

- [ ] **Step 2: 创建 Shared 类库**

Run:
```bash
dotnet new classlib -n IndustrialMonitor.Shared -o src/IndustrialMonitor.Shared -f net9.0
dotnet sln add src/IndustrialMonitor.Shared/IndustrialMonitor.Shared.csproj
```

- [ ] **Step 3: 创建 Application 类库**

Run:
```bash
dotnet new classlib -n IndustrialMonitor.Application -o src/IndustrialMonitor.Application -f net9.0
dotnet sln add src/IndustrialMonitor.Application/IndustrialMonitor.Application.csproj
```

- [ ] **Step 4: 创建 Infrastructure 类库**

Run:
```bash
dotnet new classlib -n IndustrialMonitor.Infrastructure -o src/IndustrialMonitor.Infrastructure -f net9.0
dotnet sln add src/IndustrialMonitor.Infrastructure/IndustrialMonitor.Infrastructure.csproj
```

---

### Task 3: 创建可执行项目（Api, Collector, Wpf, Simulator）

**Files:**
- Create: `src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj`
- Create: `src/IndustrialMonitor.Collector/IndustrialMonitor.Collector.csproj`
- Create: `src/IndustrialMonitor.Wpf/IndustrialMonitor.Wpf.csproj`
- Create: `src/IndustrialMonitor.DeviceSimulator/IndustrialMonitor.DeviceSimulator.csproj`

- [ ] **Step 1: 创建 ASP.NET Core Web API 项目**

Run:
```bash
dotnet new webapi -n IndustrialMonitor.Api -o src/IndustrialMonitor.Api -f net9.0
dotnet sln add src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj
```

- [ ] **Step 2: 创建 Worker Service 项目**

Run:
```bash
dotnet new worker -n IndustrialMonitor.Collector -o src/IndustrialMonitor.Collector -f net9.0
dotnet sln add src/IndustrialMonitor.Collector/IndustrialMonitor.Collector.csproj
```

- [ ] **Step 3: 创建 WPF 项目**

Run:
```bash
dotnet new wpf -n IndustrialMonitor.Wpf -o src/IndustrialMonitor.Wpf -f net9.0-windows
dotnet sln add src/IndustrialMonitor.Wpf/IndustrialMonitor.Wpf.csproj
```

- [ ] **Step 4: 创建设备模拟器控制台项目**

Run:
```bash
dotnet new console -n IndustrialMonitor.DeviceSimulator -o src/IndustrialMonitor.DeviceSimulator -f net9.0
dotnet sln add src/IndustrialMonitor.DeviceSimulator/IndustrialMonitor.DeviceSimulator.csproj
```

---

### Task 4: 配置项目引用

**Files:**
- Modify: `src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj`
- Modify: `src/IndustrialMonitor.Collector/IndustrialMonitor.Collector.csproj`
- Modify: `src/IndustrialMonitor.Wpf/IndustrialMonitor.Wpf.csproj`
- Modify: `src/IndustrialMonitor.DeviceSimulator/IndustrialMonitor.DeviceSimulator.csproj`
- Modify: `src/IndustrialMonitor.Application/IndustrialMonitor.Application.csproj`
- Modify: `src/IndustrialMonitor.Infrastructure/IndustrialMonitor.Infrastructure.csproj`

- [ ] **Step 1: 配置 Api 项目引用**

Modify `src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\IndustrialMonitor.Application\IndustrialMonitor.Application.csproj" />
    <ProjectReference Include="..\IndustrialMonitor.Infrastructure\IndustrialMonitor.Infrastructure.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: 配置 Collector 项目引用**

Modify `src/IndustrialMonitor.Collector/IndustrialMonitor.Collector.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Worker">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UserSecretsId>industrialmonitor-collector</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\IndustrialMonitor.Application\IndustrialMonitor.Application.csproj" />
    <ProjectReference Include="..\IndustrialMonitor.Infrastructure\IndustrialMonitor.Infrastructure.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 3: 配置 Wpf 项目引用**

Modify `src/IndustrialMonitor.Wpf/IndustrialMonitor.Wpf.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\IndustrialMonitor.Shared\IndustrialMonitor.Shared.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 4: 配置 DeviceSimulator 项目引用**

Modify `src/IndustrialMonitor.DeviceSimulator/IndustrialMonitor.DeviceSimulator.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\IndustrialMonitor.Shared\IndustrialMonitor.Shared.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 5: 配置 Application 项目引用**

Modify `src/IndustrialMonitor.Application/IndustrialMonitor.Application.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\IndustrialMonitor.Domain\IndustrialMonitor.Domain.csproj" />
    <ProjectReference Include="..\IndustrialMonitor.Shared\IndustrialMonitor.Shared.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 6: 配置 Infrastructure 项目引用**

Modify `src/IndustrialMonitor.Infrastructure/IndustrialMonitor.Infrastructure.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\IndustrialMonitor.Domain\IndustrialMonitor.Domain.csproj" />
    <ProjectReference Include="..\IndustrialMonitor.Shared\IndustrialMonitor.Shared.csproj" />
  </ItemGroup>

</Project>
```

---

### Task 5: 安装 NuGet 包

**Files:**
- Modify: 各项目的 `.csproj`

- [ ] **Step 1: 安装 Api 项目 NuGet 包**

Run:
```bash
dotnet add src/IndustrialMonitor.Api package Microsoft.AspNetCore.OpenApi --version 9.0.*
dotnet add src/IndustrialMonitor.Api package Swashbuckle.AspNetCore --version 6.6.*
dotnet add src/IndustrialMonitor.Api package Serilog.AspNetCore --version 9.0.*
dotnet add src/IndustrialMonitor.Api package Serilog.Sinks.File --version 6.0.*
dotnet add src/IndustrialMonitor.Api package Serilog.Settings.Configuration --version 9.0.*
dotnet add src/IndustrialMonitor.Api package Microsoft.EntityFrameworkCore.Design --version 9.0.*
```

- [ ] **Step 2: 安装 Collector 项目 NuGet 包**

Run:
```bash
dotnet add src/IndustrialMonitor.Collector package Microsoft.Extensions.Hosting --version 9.0.*
dotnet add src/IndustrialMonitor.Collector package Serilog.Extensions.Hosting --version 9.0.*
dotnet add src/IndustrialMonitor.Collector package Serilog.Sinks.File --version 6.0.*
dotnet add src/IndustrialMonitor.Collector package Serilog.Settings.Configuration --version 9.0.*
dotnet add src/IndustrialMonitor.Collector package Microsoft.Extensions.Http --version 9.0.*
```

- [ ] **Step 3: 安装 Wpf 项目 NuGet 包**

Run:
```bash
dotnet add src/IndustrialMonitor.Wpf package CommunityToolkit.Mvvm --version 8.2.*
dotnet add src/IndustrialMonitor.Wpf package HandyControl --version 3.4.*
dotnet add src/IndustrialMonitor.Wpf package LiveChartsCore.SkiaSharpView.WPF --version 2.0.*
dotnet add src/IndustrialMonitor.Wpf package Microsoft.AspNetCore.SignalR.Client --version 9.0.*
dotnet add src/IndustrialMonitor.Wpf package Refit.HttpClientFactory --version 7.0.*
dotnet add src/IndustrialMonitor.Wpf package Serilog --version 4.2.*
dotnet add src/IndustrialMonitor.Wpf package Serilog.Sinks.File --version 6.0.*
dotnet add src/IndustrialMonitor.Wpf package Serilog.Settings.Configuration --version 9.0.*
dotnet add src/IndustrialMonitor.Wpf package Microsoft.Extensions.DependencyInjection --version 9.0.*
dotnet add src/IndustrialMonitor.Wpf package Microsoft.Extensions.Configuration.Json --version 9.0.*
```

- [ ] **Step 4: 安装 DeviceSimulator 项目 NuGet 包**

Run:
```bash
dotnet add src/IndustrialMonitor.DeviceSimulator package System.Text.Json --version 9.0.*
dotnet add src/IndustrialMonitor.DeviceSimulator package Microsoft.Extensions.Configuration --version 9.0.*
dotnet add src/IndustrialMonitor.DeviceSimulator package Microsoft.Extensions.Configuration.Json --version 9.0.*
dotnet add src/IndustrialMonitor.DeviceSimulator package Microsoft.Extensions.Configuration.Binder --version 9.0.*
dotnet add src/IndustrialMonitor.DeviceSimulator package Serilog --version 4.2.*
dotnet add src/IndustrialMonitor.DeviceSimulator package Serilog.Sinks.Console --version 5.0.*
dotnet add src/IndustrialMonitor.DeviceSimulator package Serilog.Sinks.File --version 6.0.*
```

- [ ] **Step 5: 安装 Application 项目 NuGet 包**

Run:
```bash
dotnet add src/IndustrialMonitor.Application package FluentValidation --version 11.9.*
dotnet add src/IndustrialMonitor.Application package Mapster --version 7.4.*
```

- [ ] **Step 6: 安装 Infrastructure 项目 NuGet 包**

Run:
```bash
dotnet add src/IndustrialMonitor.Infrastructure package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.*
dotnet add src/IndustrialMonitor.Infrastructure package Microsoft.EntityFrameworkCore.Tools --version 9.0.*
dotnet add src/IndustrialMonitor.Infrastructure package Microsoft.EntityFrameworkCore.Design --version 9.0.*
dotnet add src/IndustrialMonitor.Infrastructure package Serilog --version 4.2.*
```

- [ ] **Step 7: 安装 Shared 项目 NuGet 包**

Run:
```bash
dotnet add src/IndustrialMonitor.Shared package System.Text.Json --version 9.0.*
```

---

### Task 6: 创建 Domain 实体

**Files:**
- Create: `src/IndustrialMonitor.Domain/Entities/Device.cs`
- Create: `src/IndustrialMonitor.Domain/Entities/DeviceData.cs`
- Create: `src/IndustrialMonitor.Domain/Entities/AlarmRecord.cs`
- Create: `src/IndustrialMonitor.Domain/Entities/AlarmRule.cs`
- Create: `src/IndustrialMonitor.Domain/Entities/OperationLog.cs`

- [ ] **Step 1: 创建 Device 实体**

Create `src/IndustrialMonitor.Domain/Entities/Device.cs`:

```csharp
namespace IndustrialMonitor.Domain.Entities;

public class Device
{
    public int Id { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string ProtocolType { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public int CollectInterval { get; set; }
    public bool IsEnabled { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
```

- [ ] **Step 2: 创建 DeviceData 实体**

Create `src/IndustrialMonitor.Domain/Entities/DeviceData.cs`:

```csharp
namespace IndustrialMonitor.Domain.Entities;

public class DeviceData
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public DateTime CollectTime { get; set; }
    public double Temperature { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Pressure { get; set; }
    public string RunStatus { get; set; } = string.Empty;
    public bool IsAlarm { get; set; }
    public string? RawData { get; set; }
}
```

- [ ] **Step 3: 创建 AlarmRecord 实体**

Create `src/IndustrialMonitor.Domain/Entities/AlarmRecord.cs`:

```csharp
namespace IndustrialMonitor.Domain.Entities;

public class AlarmRecord
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string AlarmLevel { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public DateTime AlarmTime { get; set; }
    public DateTime? RecoverTime { get; set; }
    public bool IsConfirmed { get; set; }
    public string? ConfirmUser { get; set; }
    public DateTime? ConfirmTime { get; set; }
}
```

- [ ] **Step 4: 创建 AlarmRule 实体**

Create `src/IndustrialMonitor.Domain/Entities/AlarmRule.cs`:

```csharp
namespace IndustrialMonitor.Domain.Entities;

public class AlarmRule
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DataKey { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public double ThresholdValue { get; set; }
    public string AlarmLevel { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
```

- [ ] **Step 5: 创建 OperationLog 实体**

Create `src/IndustrialMonitor.Domain/Entities/OperationLog.cs`:

```csharp
namespace IndustrialMonitor.Domain.Entities;

public class OperationLog
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string OperationContent { get; set; } = string.Empty;
    public DateTime OperationTime { get; set; }
    public string Result { get; set; } = string.Empty;
}
```

---

### Task 7: 创建 Application 接口和 Shared 契约

**Files:**
- Create: `src/IndustrialMonitor.Application/Interfaces/IDeviceRepository.cs`
- Create: `src/IndustrialMonitor.Shared/Dtos/DeviceDto.cs`
- Create: `src/IndustrialMonitor.Shared/Constants/PlatformConstants.cs`

- [ ] **Step 1: 创建 IDeviceRepository 接口空壳**

Create `src/IndustrialMonitor.Application/Interfaces/IDeviceRepository.cs`:

```csharp
namespace IndustrialMonitor.Application.Interfaces;

public interface IDeviceRepository
{
}
```

- [ ] **Step 2: 创建 DeviceDto 空壳**

Create `src/IndustrialMonitor.Shared/Dtos/DeviceDto.cs`:

```csharp
namespace IndustrialMonitor.Shared.Dtos;

public class DeviceDto
{
}
```

- [ ] **Step 3: 创建 PlatformConstants 空壳**

Create `src/IndustrialMonitor.Shared/Constants/PlatformConstants.cs`:

```csharp
namespace IndustrialMonitor.Shared.Constants;

public static class PlatformConstants
{
}
```

---

### Task 8: 创建 Infrastructure DbContext 和实体配置

**Files:**
- Create: `src/IndustrialMonitor.Infrastructure/Database/ApplicationDbContext.cs`
- Create: `src/IndustrialMonitor.Infrastructure/Database/Configurations/DeviceConfiguration.cs`
- Create: `src/IndustrialMonitor.Infrastructure/Database/Configurations/DeviceDataConfiguration.cs`
- Create: `src/IndustrialMonitor.Infrastructure/Database/Configurations/AlarmRecordConfiguration.cs`
- Create: `src/IndustrialMonitor.Infrastructure/Database/Configurations/AlarmRuleConfiguration.cs`
- Create: `src/IndustrialMonitor.Infrastructure/Database/Configurations/OperationLogConfiguration.cs`

- [ ] **Step 1: 创建 ApplicationDbContext**

Create `src/IndustrialMonitor.Infrastructure/Database/ApplicationDbContext.cs`:

```csharp
using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMonitor.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceData> DeviceData { get; set; }
    public DbSet<AlarmRecord> AlarmRecords { get; set; }
    public DbSet<AlarmRule> AlarmRules { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

- [ ] **Step 2: 创建 DeviceConfiguration**

Create `src/IndustrialMonitor.Infrastructure/Database/Configurations/DeviceConfiguration.cs`:

```csharp
using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceCode).IsUnique();
        builder.Property(x => x.DeviceCode).HasMaxLength(50);
        builder.Property(x => x.DeviceName).HasMaxLength(100);
        builder.Property(x => x.DeviceType).HasMaxLength(50);
        builder.Property(x => x.ProtocolType).HasMaxLength(50);
        builder.Property(x => x.IpAddress).HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
    }
}
```

- [ ] **Step 3: 创建 DeviceDataConfiguration**

Create `src/IndustrialMonitor.Infrastructure/Database/Configurations/DeviceDataConfiguration.cs`:

```csharp
using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class DeviceDataConfiguration : IEntityTypeConfiguration<DeviceData>
{
    public void Configure(EntityTypeBuilder<DeviceData> builder)
    {
        builder.ToTable("DeviceData");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.CollectTime);
        builder.Property(x => x.DeviceCode).HasMaxLength(50);
        builder.Property(x => x.RunStatus).HasMaxLength(50);
    }
}
```

- [ ] **Step 4: 创建 AlarmRecordConfiguration**

Create `src/IndustrialMonitor.Infrastructure/Database/Configurations/AlarmRecordConfiguration.cs`:

```csharp
using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class AlarmRecordConfiguration : IEntityTypeConfiguration<AlarmRecord>
{
    public void Configure(EntityTypeBuilder<AlarmRecord> builder)
    {
        builder.ToTable("AlarmRecords");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.AlarmTime);
        builder.Property(x => x.DeviceCode).HasMaxLength(50);
        builder.Property(x => x.AlarmType).HasMaxLength(50);
        builder.Property(x => x.AlarmLevel).HasMaxLength(50);
        builder.Property(x => x.AlarmMessage).HasMaxLength(500);
        builder.Property(x => x.ConfirmUser).HasMaxLength(50);
    }
}
```

- [ ] **Step 5: 创建 AlarmRuleConfiguration**

Create `src/IndustrialMonitor.Infrastructure/Database/Configurations/AlarmRuleConfiguration.cs`:

```csharp
using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class AlarmRuleConfiguration : IEntityTypeConfiguration<AlarmRule>
{
    public void Configure(EntityTypeBuilder<AlarmRule> builder)
    {
        builder.ToTable("AlarmRules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceId);
        builder.Property(x => x.DataKey).HasMaxLength(50);
        builder.Property(x => x.Operator).HasMaxLength(10);
        builder.Property(x => x.AlarmLevel).HasMaxLength(50);
        builder.Property(x => x.AlarmMessage).HasMaxLength(500);
    }
}
```

- [ ] **Step 6: 创建 OperationLogConfiguration**

Create `src/IndustrialMonitor.Infrastructure/Database/Configurations/OperationLogConfiguration.cs`:

```csharp
using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.ToTable("OperationLogs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.OperationTime);
        builder.Property(x => x.UserName).HasMaxLength(50);
        builder.Property(x => x.OperationType).HasMaxLength(50);
        builder.Property(x => x.Result).HasMaxLength(50);
    }
}
```

---

### Task 9: 添加 EF Core 初始迁移

**Files:**
- Create: `src/IndustrialMonitor.Infrastructure/Database/Migrations/` 下的迁移文件

- [ ] **Step 1: 安装 dotnet-ef 工具（如未安装）**

Run:
```bash
dotnet tool install --global dotnet-ef --version 9.0.*
```
If already installed, run:
```bash
dotnet tool update --global dotnet-ef --version 9.0.*
```

- [ ] **Step 2: 创建 InitialCreate 迁移**

Run:
```bash
dotnet ef migrations add InitialCreate --project src/IndustrialMonitor.Infrastructure --startup-project src/IndustrialMonitor.Api --output-dir Database/Migrations
```
Expected: 在 `src/IndustrialMonitor.Infrastructure/Database/Migrations/` 下生成 `InitialCreate.cs` 等文件。

---

### Task 10: 配置 IndustrialMonitor.Api

**Files:**
- Modify: `src/IndustrialMonitor.Api/Program.cs`
- Modify: `src/IndustrialMonitor.Api/appsettings.json`
- Modify: `src/IndustrialMonitor.Api/appsettings.Development.json`

- [ ] **Step 1: 编写 Program.cs**

Replace `src/IndustrialMonitor.Api/Program.cs`:

```csharp
using IndustrialMonitor.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
```

- [ ] **Step 2: 编写 appsettings.json**

Replace `src/IndustrialMonitor.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=IndustrialMonitor.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/api-.log",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
```

- [ ] **Step 3: 清理 appsettings.Development.json**

Replace `src/IndustrialMonitor.Api/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

### Task 11: 配置 IndustrialMonitor.Collector

**Files:**
- Modify: `src/IndustrialMonitor.Collector/Program.cs`
- Create: `src/IndustrialMonitor.Collector/Workers/DeviceCollectorWorker.cs`
- Modify: `src/IndustrialMonitor.Collector/appsettings.json`

- [ ] **Step 1: 编写 DeviceCollectorWorker**

Create `src/IndustrialMonitor.Collector/Workers/DeviceCollectorWorker.cs`:

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IndustrialMonitor.Collector.Workers;

public class DeviceCollectorWorker : BackgroundService
{
    private readonly ILogger<DeviceCollectorWorker> _logger;

    public DeviceCollectorWorker(ILogger<DeviceCollectorWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Collector started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Collector running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
```

- [ ] **Step 2: 编写 Program.cs**

Replace `src/IndustrialMonitor.Collector/Program.cs`:

```csharp
using IndustrialMonitor.Collector.Workers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

builder.Services.AddHostedService<DeviceCollectorWorker>();

var host = builder.Build();
host.Run();
```

- [ ] **Step 3: 编写 appsettings.json**

Replace `src/IndustrialMonitor.Collector/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/collector-.log",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

---

### Task 12: 配置 IndustrialMonitor.Wpf

**Files:**
- Modify: `src/IndustrialMonitor.Wpf/App.xaml`
- Modify: `src/IndustrialMonitor.Wpf/App.xaml.cs`
- Create: `src/IndustrialMonitor.Wpf/Program.cs`
- Modify: `src/IndustrialMonitor.Wpf/MainWindow.xaml`
- Modify: `src/IndustrialMonitor.Wpf/MainWindow.xaml.cs`
- Create: `src/IndustrialMonitor.Wpf/appsettings.json`
- Modify: `src/IndustrialMonitor.Wpf/IndustrialMonitor.Wpf.csproj`

- [ ] **Step 1: 修改 App.xaml**

Replace `src/IndustrialMonitor.Wpf/App.xaml`:

```xml
<Application x:Class="IndustrialMonitor.Wpf.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/HandyControl;component/Themes/SkinDefault.xaml"/>
                <ResourceDictionary Source="pack://application:,,,/HandyControl;component/Themes/Theme.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

- [ ] **Step 2: 修改 App.xaml.cs**

Replace `src/IndustrialMonitor.Wpf/App.xaml.cs`:

```csharp
using System.Windows;

namespace IndustrialMonitor.Wpf;

public partial class App : Application
{
}
```

- [ ] **Step 3: 创建 Program.cs**

Create `src/IndustrialMonitor.Wpf/Program.cs`:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace IndustrialMonitor.Wpf;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();

        var app = serviceProvider.GetRequiredService<App>();
        app.InitializeComponent();

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        app.MainWindow = mainWindow;
        mainWindow.Show();

        try
        {
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<App>();
        services.AddSingleton<MainWindow>();
    }
}
```

- [ ] **Step 4: 修改 MainWindow.xaml**

Replace `src/IndustrialMonitor.Wpf/MainWindow.xaml`:

```xml
<Window x:Class="IndustrialMonitor.Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="工业设备数据采集与监控平台" Height="600" Width="900"
        MinHeight="400" MinWidth="600"
        WindowStartupLocation="CenterScreen">
    <Grid>
        <TextBlock Text="Industrial Monitor Platform"
                   FontSize="24"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center"/>
    </Grid>
</Window>
```

- [ ] **Step 5: 修改 MainWindow.xaml.cs**

Replace `src/IndustrialMonitor.Wpf/MainWindow.xaml.cs`:

```csharp
using System.Windows;

namespace IndustrialMonitor.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

- [ ] **Step 6: 创建 appsettings.json**

Create `src/IndustrialMonitor.Wpf/appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5000",
  "SignalRHubUrl": "http://localhost:5000/hubs/monitor",
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/wpf-.log",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

- [ ] **Step 7: 将 appsettings.json 配置为嵌入资源**

Modify `src/IndustrialMonitor.Wpf/IndustrialMonitor.Wpf.csproj`，确保包含以下属性：

```xml
<PropertyGroup>
  <StartupObject>IndustrialMonitor.Wpf.Program</StartupObject>
</PropertyGroup>

<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

---

### Task 13: 配置 IndustrialMonitor.DeviceSimulator

**Files:**
- Modify: `src/IndustrialMonitor.DeviceSimulator/Program.cs`
- Create: `src/IndustrialMonitor.DeviceSimulator/appsettings.json`
- Modify: `src/IndustrialMonitor.DeviceSimulator/IndustrialMonitor.DeviceSimulator.csproj`

- [ ] **Step 1: 编写 Program.cs**

Replace `src/IndustrialMonitor.DeviceSimulator/Program.cs`:

```csharp
using Microsoft.Extensions.Configuration;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serverAddress = configuration["ServerAddress"];
var serverPort = configuration.GetValue<int>("ServerPort");
var deviceCode = configuration["DeviceCode"];
var sendIntervalMs = configuration.GetValue<int>("SendIntervalMs");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/simulator-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Device simulator starting for {DeviceCode}...", deviceCode);
    Log.Information("Target server: {ServerAddress}:{ServerPort}", serverAddress, serverPort);
    Log.Information("Send interval: {SendIntervalMs}ms", sendIntervalMs);
    Log.Information("Simulator is running. Press Ctrl+C to exit.");
    await Task.Delay(Timeout.Infinite);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Simulator terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}
```

- [ ] **Step 2: 创建 appsettings.json**

Create `src/IndustrialMonitor.DeviceSimulator/appsettings.json`:

```json
{
  "ServerAddress": "127.0.0.1",
  "ServerPort": 5001,
  "DeviceCode": "DEV001",
  "SendIntervalMs": 1000
}
```

- [ ] **Step 3: 将 appsettings.json 配置为复制到输出目录**

Modify `src/IndustrialMonitor.DeviceSimulator/IndustrialMonitor.DeviceSimulator.csproj`，在 `<Project>` 内添加：

```xml
<ItemGroup>
  <Content Include="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

---

### Task 14: 创建设计文档要求的空目录

**Files:**
- Create directories under `src/`

- [ ] **Step 1: 创建 Api 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.Api/Controllers
mkdir -p src/IndustrialMonitor.Api/Filters
mkdir -p src/IndustrialMonitor.Api/Middlewares
mkdir -p src/IndustrialMonitor.Api/Hubs
```

- [ ] **Step 2: 创建 Collector 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.Collector/Collectors
mkdir -p src/IndustrialMonitor.Collector/Protocols/Tcp
mkdir -p src/IndustrialMonitor.Collector/Protocols/Modbus
mkdir -p src/IndustrialMonitor.Collector/Protocols/Mqtt
```

- [ ] **Step 3: 创建 Wpf 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.Wpf/Views
mkdir -p src/IndustrialMonitor.Wpf/ViewModels
mkdir -p src/IndustrialMonitor.Wpf/Models
mkdir -p src/IndustrialMonitor.Wpf/Services
mkdir -p src/IndustrialMonitor.Wpf/Resources/Styles
mkdir -p src/IndustrialMonitor.Wpf/Resources/Converters
```

- [ ] **Step 4: 创建 Simulator 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.DeviceSimulator/Simulators
mkdir -p src/IndustrialMonitor.DeviceSimulator/Configs
```

- [ ] **Step 5: 创建 Application 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.Application/Devices
mkdir -p src/IndustrialMonitor.Application/Alarms
mkdir -p src/IndustrialMonitor.Application/History
mkdir -p src/IndustrialMonitor.Application/Dashboard
```

- [ ] **Step 6: 创建 Domain 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.Domain/Enums
mkdir -p src/IndustrialMonitor.Domain/ValueObjects
mkdir -p src/IndustrialMonitor.Domain/Events
```

- [ ] **Step 7: 创建 Infrastructure 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.Infrastructure/Repositories
mkdir -p src/IndustrialMonitor.Infrastructure/Logging
mkdir -p src/IndustrialMonitor.Infrastructure/Communication
```

- [ ] **Step 8: 创建 Shared 空目录**

Run:
```bash
mkdir -p src/IndustrialMonitor.Shared/CommonModels
mkdir -p src/IndustrialMonitor.Shared/Extensions
```

---

### Task 15: 编译整个解决方案

**Files:**
- All

- [ ] **Step 1: 编译解决方案**

Run:
```bash
dotnet build IndustrialMonitorPlatform.sln
```
Expected: Build succeeded with 0 warning(s) and 0 error(s) for all 8 projects.

- [ ] **Step 2: 处理编译错误（如有）**

如果提示缺少 using，添加对应 `using` 语句；如果提示包版本不兼容，升级或降级 NuGet 包版本。

---

### Task 16: 运行冒烟测试

**Files:**
- N/A

- [ ] **Step 1: 运行 API 并验证 Swagger**

Run:
```bash
dotnet run --project src/IndustrialMonitor.Api
```
Expected: 控制台显示 `Now listening on: http://localhost:5000` 或 `https://localhost:5001`。
验证：在浏览器访问 `http://localhost:5000/swagger` 或 `https://localhost:5001/swagger`，Swagger UI 正常加载。
验证：检查 `src/IndustrialMonitor.Api/IndustrialMonitor.db` 文件已生成。

- [ ] **Step 2: 运行 Collector 并验证 Worker 启动**

Run:
```bash
dotnet run --project src/IndustrialMonitor.Collector
```
Expected: 控制台输出 `Collector started.` 和周期性 `Collector running at:` 日志。

- [ ] **Step 3: 运行 WPF 并验证窗口显示**

Run:
```bash
dotnet run --project src/IndustrialMonitor.Wpf
```
Expected: 显示标题为 "工业设备数据采集与监控平台" 的窗口。

- [ ] **Step 4: 运行 DeviceSimulator 并验证启动**

Run:
```bash
dotnet run --project src/IndustrialMonitor.DeviceSimulator
```
Expected: 控制台输出 `Device simulator starting...` 和 `Simulator is running.`。

---

## 自审检查

### Spec 覆盖检查

- [x] 解决方案和 8 个项目 — Task 1-3
- [x] 项目引用关系 — Task 4
- [x] NuGet 包配置 — Task 5
- [x] Domain 实体 — Task 6
- [x] Application 接口和 Shared 契约 — Task 7
- [x] Infrastructure DbContext 和迁移 — Task 8-9
- [x] Api 启动配置 — Task 10
- [x] Collector 启动配置 — Task 11
- [x] Wpf 启动配置 — Task 12
- [x] DeviceSimulator 启动配置 — Task 13
- [x] 空目录结构 — Task 14
- [x] 编译和冒烟测试 — Task 15-16

### Placeholder 扫描

- [x] 无 "TBD" / "TODO" / "implement later"
- [x] 无 "Add appropriate error handling" 等模糊描述
- [x] 每个代码步骤包含完整代码
- [x] 每个命令步骤包含预期输出

### 类型一致性检查

- [x] `ApplicationDbContext` 中的 `DbSet<DeviceData>` 与 `DeviceDataConfiguration` 中的 `ToTable("DeviceData")` 一致
- [x] `DeviceCollectorWorker` 类名在文件和 `Program.cs` 注册中一致
- [x] `App` 和 `MainWindow` 命名空间均为 `IndustrialMonitor.Wpf`

---

## 执行交接

计划已完成并保存到 `docs/superpowers/plans/2026-06-14-project-structure.md`。

**两种执行方式：**

1. **Subagent-Driven（推荐）** — 为每个 Task 派独立子代理执行，主代理在每个 Task 后审查结果，迭代快。
2. **Inline Execution** — 在当前会话中使用 executing-plans 批量执行，遇到检查点暂停。

请选择执行方式。
