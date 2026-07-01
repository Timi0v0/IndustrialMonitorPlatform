using System.IO;
using System.Text.Json;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class ConfigurationFileStartupCheck : IStartupCheck
{
    public string Name => "Configuration file";

    public Task<StartupCheckResult> RunAsync(StartupCheckContext context, CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(context.BaseDirectory, "appsettings.json");

        if (!File.Exists(filePath))
        {
            return Task.FromResult(new StartupCheckResult(
                "Startup.Config.Missing",
                StartupCheckSeverity.Error,
                "配置文件缺失",
                "未找到 appsettings.json，软件无法读取后端地址和日志配置。",
                $"请确认安装目录中存在文件：{filePath}"));
        }

        try
        {
            using var stream = File.OpenRead(filePath);
            using var _ = JsonDocument.Parse(stream);
        }
        catch (JsonException ex)
        {
            return Task.FromResult(new StartupCheckResult(
                "Startup.Config.InvalidJson",
                StartupCheckSeverity.Error,
                "配置文件格式错误",
                "appsettings.json 不是合法的 JSON 文件。",
                ex.Message));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Task.FromResult(new StartupCheckResult(
                "Startup.Config.Unreadable",
                StartupCheckSeverity.Error,
                "配置文件无法读取",
                "当前用户没有权限读取 appsettings.json，或文件正在被占用。",
                ex.Message));
        }

        return Task.FromResult(new StartupCheckResult(
            "Startup.Config.Ok",
            StartupCheckSeverity.Info,
            "配置文件正常",
            "appsettings.json 已找到并且格式正确。"));
    }
}
