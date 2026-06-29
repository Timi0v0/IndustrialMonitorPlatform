using System.Collections.ObjectModel;
using System.Windows;
using HandyControl.Data;
using HandyControl.Tools;

namespace IndustrialMonitor.Wpf.Services;

/// <summary>
/// 主题资源切换器：负责替换 Application.Resources.MergedDictionaries 中的
/// HandyControl 皮肤/主题和自定义应用主题资源
/// </summary>
public static class ThemeResourceSwitcher
{
    /// <summary>
    /// 标记键：用于标识非 URI 方式添加的资源字典的来源
    /// </summary>
    internal const string SourceMarkerKey = "__IndustrialMonitorThemeSource";

    // 自定义应用主题资源路径
    private const string LightAppThemeUri = "/Themes/AppLight.xaml";
    private const string DarkAppThemeUri = "/Themes/AppDark.xaml";

    /// <summary>
    /// 应用指定主题：替换 MergedDictionaries 中的资源字典
    /// 顺序：[0] HandyControl 皮肤 → [1] HandyControl 主题 → [2] 自定义应用颜色
    /// </summary>
    public static void ApplyTheme(Collection<ResourceDictionary> dictionaries, AppTheme theme)
    {
        ApplyTheme(
            dictionaries,
            theme,
            () => ResourceHelper.GetSkin(GetSkinType(theme)),
            ResourceHelper.GetTheme,
            () => new ResourceDictionary { Source = new Uri(GetAppThemeUri(theme), UriKind.Relative) });
    }

    /// <summary>
    /// 内部实现：移除旧字典，按顺序插入 HandyControl 皮肤、HandyControl 主题、自定义主题
    /// </summary>
    internal static void ApplyTheme(
        Collection<ResourceDictionary> dictionaries,
        AppTheme theme,
        Func<ResourceDictionary> handySkinFactory,
        Func<ResourceDictionary> handyThemeFactory,
        Func<ResourceDictionary> appThemeFactory)
    {
        // 1. 移除旧的 HandyControl 皮肤/主题和自定义应用主题
        RemoveThemeDictionaries(dictionaries);

        // 2. 插入新的 HandyControl 皮肤（位置 0）
        dictionaries.Insert(0, handySkinFactory());

        // 3. 如果 Theme.xaml 不存在则插入（位置 1）
        if (!ContainsHandyTheme(dictionaries))
        {
            dictionaries.Insert(1, handyThemeFactory());
        }

        // 4. 添加自定义应用主题颜色（末尾，优先级最高）
        dictionaries.Add(appThemeFactory());
    }

    /// <summary>
    /// 移除字典集合中所有与主题相关的旧资源字典
    /// （HandyControl 皮肤/主题和自定义 AppLight/AppDark）
    /// </summary>
    private static void RemoveThemeDictionaries(Collection<ResourceDictionary> dictionaries)
    {
        // 倒序遍历删除，避免索引偏移
        for (var i = dictionaries.Count - 1; i >= 0; i--)
        {
            var source = GetSource(dictionaries[i]);
            if (source is null)
            {
                continue;
            }

            // 匹配 HandyControl 皮肤、自定义亮/暗主题资源
            if (source.Contains("/HandyControl;component/Themes/Skin", StringComparison.OrdinalIgnoreCase) ||
                source.Contains("/Themes/AppLight.xaml", StringComparison.OrdinalIgnoreCase) ||
                source.Contains("/Themes/AppDark.xaml", StringComparison.OrdinalIgnoreCase))
            {
                dictionaries.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 检查字典集合中是否已包含 HandyControl 主题 (Theme.xaml)
    /// 避免重复插入
    /// </summary>
    private static bool ContainsHandyTheme(Collection<ResourceDictionary> dictionaries)
    {
        return dictionaries.Any(x =>
            GetSource(x)?.Contains("/HandyControl;component/Themes/Theme.xaml", StringComparison.OrdinalIgnoreCase) == true);
    }

    /// <summary>
    /// 获取资源字典的来源标识（URI 或标记键值）
    /// </summary>
    internal static string? GetSource(ResourceDictionary dictionary)
    {
        return dictionary.Source?.OriginalString ??
               dictionary[SourceMarkerKey] as string;
    }

    /// <summary>
    /// 将应用主题枚举映射为 HandyControl 的 SkinType
    /// </summary>
    private static SkinType GetSkinType(AppTheme theme)
    {
        return theme == AppTheme.Dark ? SkinType.Dark : SkinType.Default;
    }

    /// <summary>
    /// 获取自定义应用主题资源的 URI
    /// </summary>
    private static string GetAppThemeUri(AppTheme theme)
    {
        return theme == AppTheme.Dark ? DarkAppThemeUri : LightAppThemeUri;
    }
}
