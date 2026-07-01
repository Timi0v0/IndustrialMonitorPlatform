using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace IndustrialMonitor.Wpf.Services;

public sealed class LocalizationStore : INotifyPropertyChanged
{
    private IReadOnlyDictionary<string, string> _translations =
        ReadOnlyDictionary<string, string>.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Version { get; private set; }

    public string this[string key] => GetString(key);

    public string GetString(string key)
    {
        return _translations.TryGetValue(key, out var value) ? value : $"[{key}]";
    }

    public string Format(string key, params object?[] args)
    {
        var format = GetString(key);
        try
        {
            return string.Format(CultureInfo.CurrentUICulture, format, args);
        }
        catch (FormatException)
        {
            return format;
        }
    }

    public void Update(IReadOnlyDictionary<string, string> translations)
    {
        _translations = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(translations, StringComparer.OrdinalIgnoreCase));
        Version++;
        OnPropertyChanged(Binding.IndexerName);
        OnPropertyChanged(nameof(Version));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
