using IndustrialMonitor.Wpf.StartupChecks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using Xunit;

namespace IndustrialMonitor.Wpf.Tests;

public sealed class StartupEnvironmentCheckerTests : IDisposable
{
    private readonly string _baseDirectory;

    public StartupEnvironmentCheckerTests()
    {
        _baseDirectory = Path.Combine(Path.GetTempPath(), "industrial-monitor-startup-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_baseDirectory);
    }

    [Fact]
    public async Task ConfigurationFileCheck_ReturnsErrorWhenAppsettingsMissing()
    {
        var check = new ConfigurationFileStartupCheck();

        var result = await check.RunAsync(CreateContext(), CancellationToken.None);

        Assert.Equal(StartupCheckSeverity.Error, result.Severity);
        Assert.Equal("Startup.Config.Missing", result.Key);
    }

    [Fact]
    public async Task ConfigurationFileCheck_ReturnsErrorWhenAppsettingsJsonIsInvalid()
    {
        WriteFile("appsettings.json", "{ invalid json");
        var check = new ConfigurationFileStartupCheck();

        var result = await check.RunAsync(CreateContext(), CancellationToken.None);

        Assert.Equal(StartupCheckSeverity.Error, result.Severity);
        Assert.Equal("Startup.Config.InvalidJson", result.Key);
    }

    [Fact]
    public async Task EndpointConfigurationCheck_ReturnsErrorWhenApiBaseUrlIsInvalid()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiBaseUrl"] = "not-a-url",
                ["SignalRHubUrl"] = "http://localhost:5281/hubs/monitor"
            })
            .Build();
        var check = new EndpointConfigurationStartupCheck();

        var result = await check.RunAsync(CreateContext(configuration), CancellationToken.None);

        Assert.Equal(StartupCheckSeverity.Error, result.Severity);
        Assert.Equal("Startup.Endpoint.ApiBaseUrlInvalid", result.Key);
    }

    [Fact]
    public async Task LanguageFilesCheck_ReturnsWarningWhenLanguageFileMissing()
    {
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "Languages"));
        WriteFile(Path.Combine("Languages", "zh-CN.json"), """{"App.Title":"工业监控平台"}""");
        var check = new LanguageFilesStartupCheck();

        var result = await check.RunAsync(CreateContext(), CancellationToken.None);

        Assert.Equal(StartupCheckSeverity.Warning, result.Severity);
        Assert.Equal("Startup.Language.Missing", result.Key);
    }

    [Fact]
    public async Task ApiHealthCheck_ReturnsWarningWhenApiIsUnreachable()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiBaseUrl"] = "http://127.0.0.1:1"
            })
            .Build();
        var check = new ApiHealthStartupCheck(new HttpClient(new FailingHttpMessageHandler()));

        var result = await check.RunAsync(CreateContext(configuration), CancellationToken.None);

        Assert.Equal(StartupCheckSeverity.Warning, result.Severity);
        Assert.Equal("Startup.Api.Unreachable", result.Key);
    }

    [Fact]
    public async Task StartupEnvironmentChecker_AggregatesRegisteredChecks()
    {
        var checks = new IStartupCheck[]
        {
            new FixedStartupCheck(new StartupCheckResult("Info", StartupCheckSeverity.Info, "ok", "ok")),
            new FixedStartupCheck(new StartupCheckResult("Error", StartupCheckSeverity.Error, "bad", "bad"))
        };
        var checker = new StartupEnvironmentChecker(checks, CreateContext());

        var results = await checker.RunAsync(CancellationToken.None);

        Assert.Equal(2, results.Count);
        Assert.Contains(results, x => x.Severity == StartupCheckSeverity.Error);
    }

    public void Dispose()
    {
        if (Directory.Exists(_baseDirectory))
        {
            Directory.Delete(_baseDirectory, recursive: true);
        }
    }

    private StartupCheckContext CreateContext(IConfiguration? configuration = null)
    {
        configuration ??= new ConfigurationBuilder().Build();
        return new StartupCheckContext(_baseDirectory, configuration);
    }

    private void WriteFile(string relativePath, string content)
    {
        var filePath = Path.Combine(_baseDirectory, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, content);
    }

    private sealed class FixedStartupCheck : IStartupCheck
    {
        private readonly StartupCheckResult _result;

        public FixedStartupCheck(StartupCheckResult result)
        {
            _result = result;
        }

        public string Name => _result.Key;

        public Task<StartupCheckResult> RunAsync(StartupCheckContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(_result);
        }
    }

    private sealed class FailingHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new HttpRequestException("Connection failed.");
        }
    }
}
