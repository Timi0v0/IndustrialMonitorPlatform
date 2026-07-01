using Microsoft.Extensions.Configuration;
using System.IO;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class LogDirectoryStartupCheck : IStartupCheck
{
    public string Name => "Log directory";

    public Task<StartupCheckResult> RunAsync(StartupCheckContext context, CancellationToken cancellationToken)
    {
        var logPath = context.Configuration.GetSection("Serilog:WriteTo")
            .GetChildren()
            .Select(x => x.GetValue<string>("Args:path"))
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        var logDirectory = ResolveLogDirectory(context.BaseDirectory, logPath);

        try
        {
            Directory.CreateDirectory(logDirectory);

            var probePath = Path.Combine(logDirectory, $".startup-write-test-{Guid.NewGuid():N}.tmp");
            File.WriteAllText(probePath, "startup-check");
            File.Delete(probePath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Task.FromResult(new StartupCheckResult(
                "Startup.Log.Unwritable",
                StartupCheckSeverity.Error,
                "日志目录不可写",
                "当前用户无法写入日志目录，运行问题将无法记录。",
                $"请检查目录权限：{logDirectory}。{ex.Message}"));
        }

        return Task.FromResult(new StartupCheckResult(
            "Startup.Log.Ok",
            StartupCheckSeverity.Info,
            "日志目录正常",
            $"日志目录可写：{logDirectory}"));
    }

    private static string ResolveLogDirectory(string baseDirectory, string? logPath)
    {
        if (string.IsNullOrWhiteSpace(logPath))
        {
            return Path.Combine(baseDirectory, "logs");
        }

        var normalizedPath = logPath.Replace('/', Path.DirectorySeparatorChar);
        var directory = Path.GetDirectoryName(normalizedPath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            directory = "logs";
        }

        return Path.IsPathFullyQualified(directory)
            ? directory
            : Path.GetFullPath(Path.Combine(baseDirectory, directory));
    }
}
