using System.Windows;
using IndustrialMonitor.Wpf.Services;
using IndustrialMonitor.Wpf.Views;

namespace IndustrialMonitor.Wpf;

public partial class MainWindow : Window
{
    private readonly ILocalizationService _localizationService;
    private readonly IThemeService _themeService;

    public MainWindow(
        DeviceListView deviceListView,
        IThemeService themeService,
        ILocalizationService localizationService)
    {
        _themeService = themeService;
        _localizationService = localizationService;

        InitializeComponent();
        RootContainer.Children.Add(deviceListView);
    }

    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _themeService.Toggle();
    }

    private void LanguageToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _localizationService.ToggleCulture();
    }
}
