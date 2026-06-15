# IndustrialMonitorPlatform

工业设备数据采集与监控平台。项目基于 .NET 9 搭建，目标是模拟工业现场中设备接入、数据采集、实时监控、报警处理、历史查询和第三方接口开放等典型业务链路。

当前仓库处于基础框架阶段：已经完成解决方案分层、基础实体、EF Core + SQLite、Serilog 日志、Swagger、WPF 客户端外壳、采集 Worker 和设备模拟器项目搭建。后续可在此基础上继续实现设备管理 API、真实采集协议、SignalR 推送和监控界面。

## 技术栈

| 模块 | 技术 |
| --- | --- |
| 后端 API | ASP.NET Core Web API、Swagger / OpenAPI、SignalR |
| 后台采集 | .NET Worker Service、BackgroundService |
| 桌面客户端 | WPF、CommunityToolkit.Mvvm、HandyControl、LiveCharts2、Refit |
| 数据访问 | Entity Framework Core、SQLite |
| 日志 | Serilog |
| 设备模拟 | .NET Console、JSON 配置、Serilog Console/File |
| 公共能力 | FluentValidation、Mapster、Shared DTO / Constants |

## 项目结构

```text
IndustrialMonitorPlatform
|-- IndustrialMonitorPlatform.sln
|-- README.md
|-- docs/
|   `-- superpowers/
|-- src/
|   |-- IndustrialMonitor.Api/              # ASP.NET Core API，Swagger，SQLite 迁移入口
|   |-- IndustrialMonitor.Application/      # 应用层接口、校验、映射等业务编排入口
|   |-- IndustrialMonitor.Collector/        # 后台采集 Worker
|   |-- IndustrialMonitor.DeviceSimulator/  # 设备模拟器控制台程序
|   |-- IndustrialMonitor.Domain/           # 领域实体
|   |-- IndustrialMonitor.Infrastructure/   # EF Core DbContext、配置、迁移
|   |-- IndustrialMonitor.Shared/           # DTO、常量等共享类型
|   `-- IndustrialMonitor.Wpf/              # WPF 桌面客户端
`-- 工业设备数据采集与监控平台实施方案.md
```

## 核心模型

当前已定义的领域实体包括：

| 实体 | 说明 |
| --- | --- |
| `Device` | 工业设备基础信息，如设备编码、名称、协议、IP、端口、采集周期等 |
| `DeviceData` | 设备采集数据，如温度、电压、电流、压力、运行状态和原始数据 |
| `AlarmRule` | 报警规则，如数据项、比较符、阈值、报警等级和报警信息 |
| `AlarmRecord` | 报警记录，如报警类型、报警时间、恢复时间、确认人等 |
| `OperationLog` | 操作日志，用于记录用户操作和结果 |

## 环境要求

- Windows 10/11
- .NET SDK 9.0
- Visual Studio 2022 或 Rider，需支持 .NET 9 和 WPF
- SQLite 数据库由 API 启动时自动创建，无需单独安装服务端数据库

检查 .NET SDK：

```powershell
dotnet --version
```

## 快速开始

在仓库根目录执行：

```powershell
dotnet restore
dotnet build
```

启动 API：

```powershell
dotnet run --project src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj
```

API 默认开发地址来自 `src/IndustrialMonitor.Api/Properties/launchSettings.json`：

- HTTP: `http://localhost:5281`
- HTTPS: `https://localhost:7210`
- Swagger: `http://localhost:5281/swagger`

如果希望与 WPF 默认配置保持一致，也可以用 `5000` 端口启动 API：

```powershell
dotnet run --project src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj --urls http://localhost:5000
```

启动采集 Worker：

```powershell
dotnet run --project src/IndustrialMonitor.Collector/IndustrialMonitor.Collector.csproj
```

启动设备模拟器：

```powershell
dotnet run --project src/IndustrialMonitor.DeviceSimulator/IndustrialMonitor.DeviceSimulator.csproj
```

启动 WPF 客户端：

```powershell
dotnet run --project src/IndustrialMonitor.Wpf/IndustrialMonitor.Wpf.csproj
```

也可以直接用 Visual Studio 打开 `IndustrialMonitorPlatform.sln`，将多个项目设置为同时启动。

## 配置说明

### API

配置文件：`src/IndustrialMonitor.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=IndustrialMonitor.db"
  }
}
```

API 启动时会自动执行 EF Core 迁移：

```csharp
db.Database.Migrate();
```

数据库文件会生成在 API 运行目录下。日志会输出到 `logs/api-.log` 滚动文件中。

### WPF

配置文件：`src/IndustrialMonitor.Wpf/appsettings.json`

```json
{
  "ApiBaseUrl": "http://localhost:5000",
  "SignalRHubUrl": "http://localhost:5000/hubs/monitor"
}
```

如果 API 使用默认 `5281` 端口启动，需要将这里改为：

```json
{
  "ApiBaseUrl": "http://localhost:5281",
  "SignalRHubUrl": "http://localhost:5281/hubs/monitor"
}
```

### 设备模拟器

配置文件：`src/IndustrialMonitor.DeviceSimulator/appsettings.json`

```json
{
  "ServerAddress": "127.0.0.1",
  "ServerPort": 5001,
  "DeviceCode": "DEV001",
  "SendIntervalMs": 1000
}
```

当前模拟器已读取配置并输出运行日志，后续可扩展为 TCP / Modbus / MQTT 数据上报。

## 数据库迁移

当前已有初始迁移，位于：

```text
src/IndustrialMonitor.Infrastructure/Database/Migrations/
```

如果后续实体发生变化，可在根目录执行：

```powershell
dotnet ef migrations add MigrationName `
  --project src/IndustrialMonitor.Infrastructure/IndustrialMonitor.Infrastructure.csproj `
  --startup-project src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj `
  --output-dir Database/Migrations
```

手动更新数据库：

```powershell
dotnet ef database update `
  --project src/IndustrialMonitor.Infrastructure/IndustrialMonitor.Infrastructure.csproj `
  --startup-project src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj
```

## 当前实现状态

- 已完成：解决方案和项目分层
- 已完成：Domain 实体定义
- 已完成：Infrastructure DbContext、实体配置和初始迁移
- 已完成：API 基础启动、Swagger、SQLite、Serilog
- 已完成：Collector Worker 基础循环任务
- 已完成：DeviceSimulator 配置读取和运行日志
- 已完成：WPF 启动外壳和基础窗口
- 待实现：设备管理 Controller / Repository / Service
- 待实现：设备模拟数据发送
- 待实现：采集器接收、解析、入库和报警判断
- 待实现：SignalR Hub 与 WPF 实时刷新
- 待实现：历史数据查询、报表导出和第三方接口

## 推荐开发顺序

1. 补齐设备管理 CRUD API 和仓储实现。
2. 在 WPF 中实现设备列表、新增、编辑和删除。
3. 扩展设备模拟器，定时生成并发送设备数据。
4. 扩展采集 Worker，接收模拟器数据并写入 SQLite。
5. 实现报警规则判断和报警记录生成。
6. 增加 SignalR Hub，将实时数据推送给 WPF。
7. 实现实时看板、历史查询和报表导出。

## 常见问题

### Swagger 打不开

确认 API 是以 `Development` 环境启动。`launchSettings.json` 已默认配置：

```json
"ASPNETCORE_ENVIRONMENT": "Development"
```

### WPF 连接不上 API

检查 WPF 的 `ApiBaseUrl` 是否与 API 实际端口一致。API 默认是 `5281`，WPF 默认配置是 `5000`。

### 没有看到数据库文件

数据库会在 API 启动并执行迁移后生成。请先运行：

```powershell
dotnet run --project src/IndustrialMonitor.Api/IndustrialMonitor.Api.csproj
```

### `dotnet ef` 命令不可用

安装或更新 EF Core CLI：

```powershell
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

## 相关文档

- [工业设备数据采集与监控平台实施方案.md](工业设备数据采集与监控平台实施方案.md)
- [项目结构设计](docs/superpowers/specs/2026-06-14-project-structure-design.md)
- [项目结构实施计划](docs/superpowers/plans/2026-06-14-project-structure.md)
