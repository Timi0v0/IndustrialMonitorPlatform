namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed record StartupCheckResult(
    string Key,
    StartupCheckSeverity Severity,
    string Title,
    string Message,
    string? Remedy = null);
