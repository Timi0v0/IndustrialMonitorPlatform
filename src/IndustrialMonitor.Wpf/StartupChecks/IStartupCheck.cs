namespace IndustrialMonitor.Wpf.StartupChecks;

public interface IStartupCheck
{
    string Name { get; }

    Task<StartupCheckResult> RunAsync(StartupCheckContext context, CancellationToken cancellationToken);
}
