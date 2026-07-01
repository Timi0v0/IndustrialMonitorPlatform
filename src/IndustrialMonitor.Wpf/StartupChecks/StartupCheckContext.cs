using Microsoft.Extensions.Configuration;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class StartupCheckContext
{
    public StartupCheckContext(string baseDirectory, IConfiguration configuration)
    {
        BaseDirectory = baseDirectory;
        Configuration = configuration;
    }

    public string BaseDirectory { get; }

    public IConfiguration Configuration { get; }
}
