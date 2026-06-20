using IndustrialMonitor.DeviceSimulator.Configs;
using IndustrialMonitor.DeviceSimulator.Simulators;
using Microsoft.Extensions.Configuration;
using Serilog;

// 读取 appsettings.json 反序列化为配置对象
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var simulatorConfig = configuration.Get<SimulatorRootConfig>()
                     ?? throw new InvalidOperationException("无法解析模拟器配置。");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/simulator-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// 为每台设备创建对应的模拟器实例
var simulators = new List<DeviceSimulatorBase>();
foreach (var device in simulatorConfig.Devices)
{
    DeviceSimulatorBase sim = device.EnableAbnormal
        ? new AbnormalDataSimulator(device)
        : new NormalDataSimulator(device);
    simulators.Add(sim);

    Log.Information(
        "[{DeviceCode}] 已创建模拟器 | SendInterval={SendIntervalMs}ms | Abnormal={EnableAbnormal} (p={AbnormalProbability})",
        device.DeviceCode,
        device.SendIntervalMs,
        device.EnableAbnormal,
        device.AbnormalProbability);
}

// 使用 CancellationTokenSource 实现 Ctrl+C 优雅退出
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    Log.Information("正在停止所有模拟器...");
    cts.Cancel();
};

try
{
    Log.Information("=== Device Simulator 启动 ===");
    Log.Information("目标服务器: {Address}:{Port}", simulatorConfig.ServerAddress, simulatorConfig.ServerPort);
    Log.Information("模拟设备数量: {Count}", simulators.Count);

    // 并行启动所有模拟器
    var tasks = simulators.Select(sim =>
        sim.StartAsync(simulatorConfig.ServerAddress, simulatorConfig.ServerPort, cts.Token));

    await Task.WhenAll(tasks);

    Log.Information("所有模拟器已停止。");
}
catch (OperationCanceledException)
{
    Log.Information("模拟器已取消。");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Simulator terminated unexpectedly.");
}
finally
{
    // 释放所有模拟器资源
    foreach (var sim in simulators)
    {
        sim.Dispose();
    }

    await Log.CloseAndFlushAsync();
}
