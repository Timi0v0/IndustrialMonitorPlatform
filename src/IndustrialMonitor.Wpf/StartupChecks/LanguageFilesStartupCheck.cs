using System.IO;
using System.Text.Json;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class LanguageFilesStartupCheck : IStartupCheck
{
    private static readonly string[] RequiredCultureNames = ["zh-CN", "en-US"];

    public string Name => "Language files";

    public Task<StartupCheckResult> RunAsync(StartupCheckContext context, CancellationToken cancellationToken)
    {
        var languageDirectory = Path.Combine(context.BaseDirectory, "Languages");

        if (!Directory.Exists(languageDirectory))
        {
            return Task.FromResult(new StartupCheckResult(
                "Startup.Language.DirectoryMissing",
                StartupCheckSeverity.Warning,
                "翻译目录缺失",
                "未找到 Languages 目录，界面翻译可能无法加载。",
                $"请确认安装目录中存在目录：{languageDirectory}"));
        }

        foreach (var cultureName in RequiredCultureNames)
        {
            var filePath = Path.Combine(languageDirectory, $"{cultureName}.json");

            if (!File.Exists(filePath))
            {
                return Task.FromResult(new StartupCheckResult(
                    "Startup.Language.Missing",
                    StartupCheckSeverity.Warning,
                    "翻译文件缺失",
                    $"未找到 {cultureName}.json，切换到该语言时可能显示 key。",
                    $"请确认文件存在：{filePath}"));
            }

            try
            {
                using var stream = File.OpenRead(filePath);
                using var _ = JsonDocument.Parse(stream);
            }
            catch (JsonException ex)
            {
                return Task.FromResult(new StartupCheckResult(
                    "Startup.Language.InvalidJson",
                    StartupCheckSeverity.Warning,
                    "翻译文件格式错误",
                    $"{cultureName}.json 不是合法的 JSON 文件，界面翻译可能无法刷新。",
                    ex.Message));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                return Task.FromResult(new StartupCheckResult(
                    "Startup.Language.Unreadable",
                    StartupCheckSeverity.Warning,
                    "翻译文件无法读取",
                    $"{cultureName}.json 无法读取，界面翻译可能无法加载。",
                    ex.Message));
            }
        }

        return Task.FromResult(new StartupCheckResult(
            "Startup.Language.Ok",
            StartupCheckSeverity.Info,
            "翻译文件正常",
            "中英文翻译文件已找到并且格式正确。"));
    }
}
