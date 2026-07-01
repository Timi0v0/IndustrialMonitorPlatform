using Serilog;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class StartupEnvironmentChecker : IStartupEnvironmentChecker
{
    private readonly IEnumerable<IStartupCheck> _checks;
    private readonly StartupCheckContext _context;

    public StartupEnvironmentChecker(IEnumerable<IStartupCheck> checks, StartupCheckContext context)
    {
        _checks = checks;
        _context = context;
    }

    public async Task<IReadOnlyList<StartupCheckResult>> RunAsync(CancellationToken cancellationToken)
    {
        var results = new List<StartupCheckResult>();

        foreach (var check in _checks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                results.Add(await check.RunAsync(_context, cancellationToken).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Startup check {CheckName} failed unexpectedly.", check.Name);
                results.Add(new StartupCheckResult(
                    "Startup.Check.UnexpectedError",
                    StartupCheckSeverity.Error,
                    $"{check.Name} 检查失败",
                    "启动环境检查执行时发生异常。",
                    ex.Message));
            }
        }

        LogResults(results);
        return results;
    }

    private static void LogResults(IEnumerable<StartupCheckResult> results)
    {
        foreach (var result in results)
        {
            Log.Information(
                "Startup check result: {Key}, {Severity}, {Title}, {Message}, {Remedy}",
                result.Key,
                result.Severity,
                result.Title,
                result.Message,
                result.Remedy);
        }
    }
}
