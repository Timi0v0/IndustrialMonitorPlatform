using System.Globalization;

namespace IndustrialMonitor.Wpf.Services;

public interface ILocalizationService
{
    CultureInfo CurrentCulture { get; }

    LocalizationStore Store { get; }

    void ApplyCulture(string cultureName);

    void ToggleCulture();

    string GetString(string key);

    string Format(string key, params object?[] args);
}
