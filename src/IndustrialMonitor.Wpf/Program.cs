using IndustrialMonitor.Shared.Dtos;
using IndustrialMonitor.Wpf.Services;
using IndustrialMonitor.Wpf.ViewModels;
using IndustrialMonitor.Wpf.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
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

    private static void ConfigureServices(IServiceCollection services)
    {
        // 基础服务
        services.AddSingleton<App>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<DeviceListView>();
        services.AddTransient<DeviceEditView>();

        // Refit API 客户端
        var apiBaseUrl = services.BuildServiceProvider()
            .GetRequiredService<IConfiguration>()
            .GetValue<string>("ApiBaseUrl") ?? "http://localhost:5281";
        //设置 Refit 指向的地址
        services.AddRefitClient<IDeviceApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

        // ViewModels（单例，与 Views 生命周期匹配）
        services.AddSingleton<DeviceListViewModel>();
        services.AddTransient<DeviceEditViewModel>();
    }
}
