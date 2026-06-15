using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace IndustrialMonitor.Wpf;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();

        var app = serviceProvider.GetRequiredService<App>();
        app.InitializeComponent();

        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        app.MainWindow = mainWindow;
        mainWindow.Show();

        try
        {
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
    /// <summary>
    /// ×¢²á·þÎñ
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<App>();
        services.AddSingleton<MainWindow>();
    }
}
