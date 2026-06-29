namespace IndustrialMonitor.Wpf.Services;

/// <summary>
/// 主题服务接口：定义亮色/暗色主题切换的契约
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// 当前主题
    /// </summary>
    AppTheme CurrentTheme { get; }

    /// <summary>
    /// 应用指定主题
    /// </summary>
    void Apply(AppTheme theme);

    /// <summary>
    /// 切换亮/暗主题
    /// </summary>
    void Toggle();
}
