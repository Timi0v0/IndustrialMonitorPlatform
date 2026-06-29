using System.Windows;
using IndustrialMonitor.Wpf.Services;
using IndustrialMonitor.Wpf.Views;

namespace IndustrialMonitor.Wpf;

/// <summary>
/// 应用程序主窗口，包含顶部标题栏和内容容器
/// </summary>
public partial class MainWindow : Window
{
    // 主题服务：管理亮/暗主题切换
    private readonly IThemeService _themeService;

    public MainWindow(DeviceListView deviceListView, IThemeService themeService)
    {
        _themeService = themeService;

        InitializeComponent();

        // 将设备列表页作为默认内容填充到 RootContainer 中
        RootContainer.Children.Add(deviceListView);
    }

    /// <summary>
    /// 主题切换按钮点击事件：切换亮/暗主题，并更新按钮图标
    /// </summary>
    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        // 切换主题（ThemeService 内部替换资源字典）
        _themeService.Toggle();

        // 更新按钮图标：亮色→太阳图标 ()，暗色→月亮图标 ()
        ThemeToggleButton.Content = _themeService.CurrentTheme == AppTheme.Light ? "" : "";
    }
}
