namespace IndustrialMonitor.Wpf.Services;

public static class LocalizationProvider
{
    public static LocalizationStore Store { get; } = new();

    public static ILocalizationService? Service { get; internal set; }
}
