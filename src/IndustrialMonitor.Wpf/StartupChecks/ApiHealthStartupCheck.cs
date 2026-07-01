using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class ApiHealthStartupCheck : IStartupCheck
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);
    private readonly HttpClient _httpClient;

    public ApiHealthStartupCheck()
        : this(new HttpClient { Timeout = DefaultTimeout })
    {
    }

    public ApiHealthStartupCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string Name => "API health";

    public async Task<StartupCheckResult> RunAsync(StartupCheckContext context, CancellationToken cancellationToken)
    {
        var apiBaseUrl = context.Configuration.GetValue<string>("ApiBaseUrl");
        if (!EndpointConfigurationStartupCheck.TryCreateAbsoluteHttpUri(apiBaseUrl, out var baseUri))
        {
            return new StartupCheckResult(
                "Startup.Api.Skipped",
                StartupCheckSeverity.Info,
                "跳过 API 连通性检查",
                "ApiBaseUrl 无效，已由服务地址配置检查处理。");
        }

        var healthUri = new Uri(baseUri!, "health");

        try
        {
            using var response = await _httpClient.GetAsync(healthUri, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return new StartupCheckResult(
                    "Startup.Api.Unhealthy",
                    StartupCheckSeverity.Warning,
                    "后端 API 状态异常",
                    $"健康检查地址返回 {(int)response.StatusCode} {response.ReasonPhrase}。",
                    $"请确认后端服务已启动并检查地址：{healthUri}");
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw;
            }

            return new StartupCheckResult(
                "Startup.Api.Unreachable",
                StartupCheckSeverity.Warning,
                "后端 API 无法访问",
                "当前无法访问后端健康检查地址，部分业务功能可能不可用。",
                $"请确认后端服务已启动并检查地址：{healthUri}。{ex.Message}");
        }

        return new StartupCheckResult(
            "Startup.Api.Ok",
            StartupCheckSeverity.Info,
            "后端 API 可访问",
            $"健康检查通过：{healthUri}");
    }
}
