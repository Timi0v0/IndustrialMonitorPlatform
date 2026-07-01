using IndustrialMonitor.Wpf.Services;
using Xunit;
using System.IO;

namespace IndustrialMonitor.Wpf.Tests;

public sealed class LocalizationServiceTests : IDisposable
{
    private readonly string _languageDirectory;

    public LocalizationServiceTests()
    {
        _languageDirectory = Path.Combine(Path.GetTempPath(), "industrial-monitor-i18n-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_languageDirectory);
    }

    [Fact]
    public void ApplyCulture_LoadsRequestedJsonLanguage()
    {
        WriteLanguage("zh-CN", """{"Common.Refresh":"刷新"}""");

        var service = new LocalizationService(_languageDirectory);

        service.ApplyCulture("zh-CN");

        Assert.Equal("刷新", service.GetString("Common.Refresh"));
        Assert.Equal("刷新", service.Store["Common.Refresh"]);
    }

    [Fact]
    public void ApplyCulture_CanSwitchToAnotherLanguage()
    {
        WriteLanguage("zh-CN", """{"Common.Refresh":"刷新"}""");
        WriteLanguage("en-US", """{"Common.Refresh":"Refresh"}""");
        var service = new LocalizationService(_languageDirectory);

        service.ApplyCulture("zh-CN");
        service.ApplyCulture("en-US");

        Assert.Equal("Refresh", service.GetString("Common.Refresh"));
        Assert.Equal("en-US", service.CurrentCulture.Name);
    }

    [Fact]
    public void GetString_ReturnsBracketedKeyWhenMissing()
    {
        WriteLanguage("zh-CN", """{}""");
        var service = new LocalizationService(_languageDirectory);

        service.ApplyCulture("zh-CN");

        Assert.Equal("[Device.Unknown]", service.GetString("Device.Unknown"));
    }

    [Fact]
    public void Format_ReplacesArguments()
    {
        WriteLanguage("zh-CN", """{"Device.TotalCount":"当前共 {0} 台设备"}""");
        var service = new LocalizationService(_languageDirectory);

        service.ApplyCulture("zh-CN");

        Assert.Equal("当前共 3 台设备", service.Format("Device.TotalCount", 3));
    }

    [Fact]
    public void ApplyCulture_InvalidJsonKeepsPreviousTranslations()
    {
        WriteLanguage("zh-CN", """{"Common.Refresh":"刷新"}""");
        var service = new LocalizationService(_languageDirectory);
        service.ApplyCulture("zh-CN");

        WriteLanguage("zh-CN", "{ invalid json");
        service.ApplyCulture("zh-CN");

        Assert.Equal("刷新", service.GetString("Common.Refresh"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_languageDirectory))
        {
            Directory.Delete(_languageDirectory, recursive: true);
        }
    }

    private void WriteLanguage(string cultureName, string json)
    {
        File.WriteAllText(Path.Combine(_languageDirectory, $"{cultureName}.json"), json);
    }
}
