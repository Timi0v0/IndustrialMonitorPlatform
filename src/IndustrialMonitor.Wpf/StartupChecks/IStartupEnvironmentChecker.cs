namespace IndustrialMonitor.Wpf.StartupChecks;

public interface IStartupEnvironmentChecker
{
    Task<IReadOnlyList<StartupCheckResult>> RunAsync(CancellationToken cancellationToken);
}
