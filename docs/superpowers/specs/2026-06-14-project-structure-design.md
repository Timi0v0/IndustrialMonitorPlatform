# 工业设备数据采集与监控平台 — 项目基本结构设计

## 1. 背景与目标

基于《工业设备数据采集与监控平台实施方案》，本设计聚焦**第一阶段：项目基础搭建**。

目标：
- 建立清晰、可扩展的 C/S 分层项目结构。
- 完成解决方案、8 个入口/类库项目、基础依赖配置和 SQLite 数据库迁移。
- 不实现具体业务逻辑，但确保每个入口项目能独立启动并通过第一阶段验收。

## 2. 关键决策

| 决策项 | 选择 | 说明 |
|--------|------|------|
| WPF MVVM 框架 | CommunityToolkit.Mvvm | 轻量、现代，与方案技术栈表一致 |
| WPF UI 控件库 | HandyControl | 适合工业上位机风格的暗色/卡片布局 |
| 图表库 | LiveCharts2 (SkiaSharp) | 性能好，支持实时滚动曲线 |
| 数据库 | SQLite + EF Core | 方案第一阶段推荐，便于演示；目标框架采用 .NET 9（已安装 SDK） |
| 日志 | Serilog | 后端写入文件，WPF/模拟器同样使用 |
| API 文档 | Swagger/OpenAPI | 方案推荐，便于面试演示 |
| 项目结构 | 完整 8 项目分层 | 最能体现架构能力，与实施方案一致 |

## 3. 解决方案目录结构

```text
d:\学习\IndustrialMonitorPlatform\
│
├── IndustrialMonitorPlatform.sln
├── README.md
├── docs/
│   └── superpowers/
│       └── specs/
│           └── 2026-06-14-project-structure-design.md
│
└── src/
    ├── IndustrialMonitor.Api/
    │   ├── Controllers/
    │   ├── Filters/
    │   ├── Hubs/
    │   ├── Middlewares/
    │   ├── appsettings.json
    │   ├── appsettings.Development.json
    │   └── Program.cs
    │
    ├── IndustrialMonitor.Collector/
    │   ├── Collectors/
    │   ├── Protocols/
    │   │   ├── Tcp/
    │   │   ├── Modbus/
    │   │   └── Mqtt/
    │   ├── Workers/
    │   ├── appsettings.json
    │   └── Program.cs
    │
    ├── IndustrialMonitor.Wpf/
    │   ├── Views/
    │   ├── ViewModels/
    │   ├── Models/
    │   ├── Services/
    │   ├── Resources/
    │   │   ├── Styles/
    │   │   └── Converters/
    │   ├── App.xaml
    │   ├── App.xaml.cs
    │   └── MainWindow.xaml
    │
    ├── IndustrialMonitor.DeviceSimulator/
    │   ├── Simulators/
    │   ├── Configs/
    │   └── Program.cs
    │
    ├── IndustrialMonitor.Application/
    │   ├── Devices/
    │   ├── Alarms/
    │   ├── History/
    │   ├── Dashboard/
    │   └── Interfaces/
    │
    ├── IndustrialMonitor.Domain/
    │   ├── Entities/
    │   ├── Enums/
    │   ├── ValueObjects/
    │   └── Events/
    │
    ├── IndustrialMonitor.Infrastructure/
    │   ├── Database/
    │   │   ├── Configurations/
    │   │   └── Migrations/
    │   ├── Repositories/
    │   ├── Logging/
    │   └── Communication/
    │
    └── IndustrialMonitor.Shared/
        ├── Dtos/
        ├── Constants/
        ├── CommonModels/
        └── Extensions/
```

第一阶段每个文件夹只建立空目录和必要的启动文件，不填充业务代码。

## 4. 项目间引用关系

```text
IndustrialMonitor.Api
    ├── IndustrialMonitor.Application
    └── IndustrialMonitor.Infrastructure

IndustrialMonitor.Collector
    ├── IndustrialMonitor.Application
    └── IndustrialMonitor.Infrastructure

IndustrialMonitor.Wpf
    └── IndustrialMonitor.Shared

IndustrialMonitor.DeviceSimulator
    └── IndustrialMonitor.Shared

IndustrialMonitor.Application
    ├── IndustrialMonitor.Domain
    └── IndustrialMonitor.Shared

IndustrialMonitor.Infrastructure
    ├── IndustrialMonitor.Domain
    └── IndustrialMonitor.Shared

IndustrialMonitor.Shared
    └── （无项目引用）

IndustrialMonitor.Domain
    └── （无项目引用）
```

## 5. 基础 NuGet 包配置

| 项目 | 基础 NuGet 包 |
|------|--------------|
| `IndustrialMonitor.Api` | `Microsoft.AspNetCore.OpenApi`, `Swashbuckle.AspNetCore`, `Serilog.AspNetCore`, `Serilog.Sinks.File`, `Microsoft.AspNetCore.SignalR` |
| `IndustrialMonitor.Collector` | `Microsoft.Extensions.Hosting`, `Serilog.Extensions.Hosting`, `Serilog.Sinks.File`, `Microsoft.Extensions.Http` |
| `IndustrialMonitor.Wpf` | `CommunityToolkit.Mvvm`, `HandyControl`, `LiveChartsCore.SkiaSharpView.WPF`, `Microsoft.AspNetCore.SignalR.Client`, `Refit.HttpClientFactory`, `Serilog`, `Serilog.Sinks.File` |
| `IndustrialMonitor.DeviceSimulator` | `System.Text.Json`, `Serilog`, `Serilog.Sinks.Console` |
| `IndustrialMonitor.Application` | `FluentValidation`, `Mapster` |
| `IndustrialMonitor.Domain` | （无第三方包） |
| `IndustrialMonitor.Infrastructure` | `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Tools`, `Serilog` |
| `IndustrialMonitor.Shared` | `System.Text.Json` |

## 6. 各项目启动配置要点

### 6.1 IndustrialMonitor.Api

- `appsettings.json`：SQLite 连接字符串、Serilog、Swagger、SignalR、CORS。
- `Program.cs`：注册 DbContext、Serilog、Swagger、SignalR、Application/Infrastructure 服务；启用全局异常处理中间件（空壳）。

### 6.2 IndustrialMonitor.Collector

- `appsettings.json`：日志、采集服务参数、目标 Api 地址。
- `Program.cs`：注册 IHostedService Worker、Serilog、Application/Infrastructure 服务；创建 Host。

### 6.3 IndustrialMonitor.Wpf

- `App.xaml`：配置 HandyControl 主题资源、主窗口。
- `appsettings.json`（嵌入资源）：API 地址、SignalR Hub 地址、日志路径。
- `App.xaml.cs`：初始化 Serilog，配置 Microsoft.Extensions.DependencyInjection 容器。

### 6.4 IndustrialMonitor.DeviceSimulator

- `appsettings.json`：服务端地址、模拟设备编号、发送间隔。
- `Program.cs`：启动 TCP 客户端模拟器结构，循环体为空。

### 6.5 IndustrialMonitor.Infrastructure

- `ApplicationDbContext`：继承 `DbContext`，暴露 `DbSet<T>`。
- 实体配置：使用 `IEntityTypeConfiguration<T>` 配置表名、字段、索引。
- 初始迁移：`InitialCreate`，包含 5 张表。

### 6.6 IndustrialMonitor.Application / Domain / Shared

- `Application`：定义仓储接口和应用服务接口空壳。
- `Domain`：定义 5 个实体空壳类（属性完整，无业务方法）。
- `Shared`：定义 DTO、常量、通用模型空目录。

## 7. 第一阶段数据库实体

实施方案中的 5 张表在第一阶段建立实体和迁移：

- `Device`
- `DeviceData`
- `AlarmRecord`
- `AlarmRule`
- `OperationLog`

实体只包含属性定义和基础数据注解，不实现业务行为。

## 8. 验收标准

1. 解决方案能正常加载，8 个项目无编译错误。
2. `IndustrialMonitor.Api` 能启动，Swagger 页面可访问。
3. `IndustrialMonitor.Collector` 能作为 Worker 启动。
4. `IndustrialMonitor.Wpf` 能启动并显示主窗口。
5. `IndustrialMonitor.DeviceSimulator` 能启动。
6. SQLite 数据库文件通过 EF Core 迁移自动生成。
7. 日志文件能正常输出。

## 9. 不在本阶段实现的内容

- 设备管理 CRUD 业务逻辑
- TCP 设备模拟器数据生成与发送
- 后台采集服务的实际采集循环
- SignalR 实时数据推送逻辑
- 报警判断与记录生成
- WPF 页面与 ViewModel 具体实现
- 历史查询与报表导出
- 第三方 Integration API 业务逻辑

---

*设计基于《工业设备数据采集与监控平台实施方案》第一阶段。*
