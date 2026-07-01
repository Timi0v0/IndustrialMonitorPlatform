using IndustrialMonitor.Shared.Dtos;
using IndustrialMonitor.Wpf.Services;
using IndustrialMonitor.Wpf.StartupChecks;
using IndustrialMonitor.Wpf.ViewModels;
using IndustrialMonitor.Wpf.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Serilog;
using System.IO;
using System.Linq;
using System.Threading;

namespace IndustrialMonitor.Wpf;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var app = new App();
        app.InitializeComponent();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.File(
                Path.Combine(AppContext.BaseDirectory, "logs", "startup-check-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddSingleton(app);
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton(new StartupCheckContext(AppContext.BaseDirectory, configuration));
        ConfigureServices(services, configuration);

        var serviceProvider = services.BuildServiceProvider();

        try
        {
            if (!RunStartupChecks(serviceProvider))
            {
                return;
            }

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            app.MainWindow = mainWindow;
            mainWindow.Show();
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
            serviceProvider.Dispose();
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IStartupEnvironmentChecker, StartupEnvironmentChecker>();
        services.AddSingleton<IStartupCheck, ConfigurationFileStartupCheck>();
        services.AddSingleton<IStartupCheck, LanguageFilesStartupCheck>();
        services.AddSingleton<IStartupCheck, LogDirectoryStartupCheck>();
        services.AddSingleton<IStartupCheck, EndpointConfigurationStartupCheck>();
        services.AddSingleton<IStartupCheck, ApiHealthStartupCheck>();

        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<DeviceListView>();
        services.AddTransient<DeviceEditView>();

        var apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
        if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiBaseUri))
        {
            apiBaseUri = new Uri("http://localhost:5281");
        }

        services.AddRefitClient<IDeviceApi>()
            .ConfigureHttpClient(c => c.BaseAddress = apiBaseUri);

        services.AddSingleton<DeviceListViewModel>();
        services.AddTransient<DeviceEditViewModel>();
    }

    private static bool RunStartupChecks(ServiceProvider serviceProvider)
    {
        while (true)
        {
            var checker = serviceProvider.GetRequiredService<IStartupEnvironmentChecker>();
            var results = checker.RunAsync(CancellationToken.None).GetAwaiter().GetResult();
            var hasError = results.Any(x => x.Severity == StartupCheckSeverity.Error);
            var hasWarning = results.Any(x => x.Severity == StartupCheckSeverity.Warning);

            if (!hasError && !hasWarning)
            {
                return true;
            }

            var dialog = new StartupCheckWindow(results, allowContinue: !hasError);
            var dialogResult = dialog.ShowDialog();

            if (dialog.RetryRequested)
            {
                continue;
            }

            return dialogResult == true && !hasError;
        }
    }
}
