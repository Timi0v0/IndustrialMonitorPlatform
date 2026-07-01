using Microsoft.Extensions.Configuration;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class EndpointConfigurationStartupCheck : IStartupCheck
{
    public string Name => "Endpoint configuration";

    public Task<StartupCheckResult> RunAsync(StartupCheckContext context, CancellationToken cancellationToken)
    {
        var apiBaseUrl = context.Configuration.GetValue<string>("ApiBaseUrl");
        if (!TryCreateAbsoluteHttpUri(apiBaseUrl, out _))
        {
            return Task.FromResult(new StartupCheckResult(
                "Startup.Endpoint.ApiBaseUrlInvalid",
                StartupCheckSeverity.Error,
                "后端 API 地址无效",
                "ApiBaseUrl 必须是合法的 http 或 https 地址。",
                "请检查 appsettings.json 中的 ApiBaseUrl。"));
        }

        var signalRHubUrl = context.Configuration.GetValue<string>("SignalRHubUrl");
        if (!TryCreateAbsoluteHttpUri(signalRHubUrl, out _))
        {
            return Task.FromResult(new StartupCheckResult(
                "Startup.Endpoint.SignalRHubUrlInvalid",
                StartupCheckSeverity.Error,
                "SignalR 地址无效",
                "SignalRHubUrl 必须是合法的 http 或 https 地址。",
                "请检查 appsettings.json 中的 SignalRHubUrl。"));
        }

        return Task.FromResult(new StartupCheckResult(
            "Startup.Endpoint.Ok",
            StartupCheckSeverity.Info,
            "服务地址配置正常",
            "ApiBaseUrl 和 SignalRHubUrl 均为合法地址。"));
    }

    internal static bool TryCreateAbsoluteHttpUri(string? value, out Uri? uri)
    {
        uri = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var parsed))
        {
            return false;
        }

        if (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        uri = parsed;
        return true;
    }
}
