using System;
using System.Windows;
using IndustrialMonitor.Wpf.Services;
using Xunit;

namespace IndustrialMonitor.Wpf.Tests;

public sealed class ThemeResourceSwitcherTests
{
    [Fact]
    public void ApplyTheme_ReplacesExistingAppThemeAndPreservesUnrelatedDictionaries()
    {
        var resources = new ResourceDictionary();
        resources.MergedDictionaries.Add(DictionaryWithSource("/Themes/AppLight.xaml"));
        resources.MergedDictionaries.Add(DictionaryWithSource("/Themes/FeatureStyles.xaml"));

        ApplyThemeForTest(resources, AppTheme.Dark, "/Themes/AppDark.xaml");

        Assert.DoesNotContain(resources.MergedDictionaries, x =>
            ThemeResourceSwitcher.GetSource(x)?.Contains("AppLight.xaml", StringComparison.OrdinalIgnoreCase) == true);
        Assert.Contains(resources.MergedDictionaries, x =>
            ThemeResourceSwitcher.GetSource(x)?.Contains("AppDark.xaml", StringComparison.OrdinalIgnoreCase) == true);
        Assert.Contains(resources.MergedDictionaries, x =>
            ThemeResourceSwitcher.GetSource(x)?.Contains("FeatureStyles.xaml", StringComparison.OrdinalIgnoreCase) == true);
    }

    [Fact]
    public void ApplyTheme_CanSwitchBackToLight()
    {
        var resources = new ResourceDictionary();
        resources.MergedDictionaries.Add(DictionaryWithSource("/Themes/AppDark.xaml"));

        ApplyThemeForTest(resources, AppTheme.Light, "/Themes/AppLight.xaml");

        Assert.DoesNotContain(resources.MergedDictionaries, x =>
            ThemeResourceSwitcher.GetSource(x)?.Contains("AppDark.xaml", StringComparison.OrdinalIgnoreCase) == true);
        Assert.Contains(resources.MergedDictionaries, x =>
            ThemeResourceSwitcher.GetSource(x)?.Contains("AppLight.xaml", StringComparison.OrdinalIgnoreCase) == true);
    }

    private static void ApplyThemeForTest(ResourceDictionary resources, AppTheme theme, string appThemeSource)
    {
        ThemeResourceSwitcher.ApplyTheme(
            resources.MergedDictionaries,
            theme,
            () => DictionaryWithSource("/HandyControl;component/Themes/Skin.xaml"),
            () => DictionaryWithSource("/HandyControl;component/Themes/Theme.xaml"),
            () => DictionaryWithSource(appThemeSource));
    }

    private static ResourceDictionary DictionaryWithSource(string source)
    {
        return new ResourceDictionary
        {
            [ThemeResourceSwitcher.SourceMarkerKey] = source
        };
    }
}
