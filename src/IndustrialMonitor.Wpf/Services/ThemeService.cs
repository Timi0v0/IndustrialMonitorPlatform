using System.Windows;

namespace IndustrialMonitor.Wpf.Services;

/// <summary>
/// 主题服务：管理亮色/暗色主题的切换
/// </summary>
public sealed class ThemeService : IThemeService
{
    /// <summary>
    /// 当前主题（默认亮色）
    /// </summary>
    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    /// <summary>
    /// 应用指定主题：替换应用程序级别的资源字典
    /// </summary>
    public void Apply(AppTheme theme)
    {
        // 替换 Application.Resources.MergedDictionaries 中的主题资源
        ThemeResourceSwitcher.ApplyTheme(Application.Current.Resources.MergedDictionaries, theme);

        CurrentTheme = theme;
    }

    /// <summary>
    /// 在亮/暗主题之间切换
    /// </summary>
    public void Toggle()
    {
        Apply(CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);
    }
}
