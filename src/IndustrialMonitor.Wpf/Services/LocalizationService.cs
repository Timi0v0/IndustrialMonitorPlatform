using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using Serilog;

namespace IndustrialMonitor.Wpf.Services;

public sealed class LocalizationService : ILocalizationService, IDisposable
{
    private const string DefaultCultureName = "zh-CN";
    private const int ReloadDebounceMilliseconds = 250;

    private readonly string _languageDirectory;
    private readonly bool _enableWatcher;
    private readonly object _reloadLock = new();
    private FileSystemWatcher? _watcher;
    private Timer? _reloadTimer;

    public LocalizationService()
        : this(Path.Combine(AppContext.BaseDirectory, "Languages"), enableWatcher: true)
    {
    }

    public LocalizationService(string languageDirectory)
        : this(languageDirectory, enableWatcher: false)
    {
    }

    private LocalizationService(string languageDirectory, bool enableWatcher)
    {
        _languageDirectory = languageDirectory;
        _enableWatcher = enableWatcher;
        Store = LocalizationProvider.Store;
        LocalizationProvider.Service = this;
        ApplyCulture(DefaultCultureName);
    }

    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo(DefaultCultureName);

    public LocalizationStore Store { get; }

    public void ApplyCulture(string cultureName)
    {
        lock (_reloadLock)
        {
            if (!TryLoadCulture(cultureName, out var translations))
            {
                return;
            }

            var culture = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CurrentCulture = culture;
            Store.Update(translations);

            if (_enableWatcher)
            {
                ResetWatcher(cultureName);
            }
        }
    }

    public void ToggleCulture()
    {
        ApplyCulture(CurrentCulture.Name.Equals("zh-CN", StringComparison.OrdinalIgnoreCase) ? "en-US" : "zh-CN");
    }

    public string GetString(string key)
    {
        return Store.GetString(key);
    }

    public string Format(string key, params object?[] args)
    {
        return Store.Format(key, args);
    }

    public void Dispose()
    {
        _reloadTimer?.Dispose();
        _watcher?.Dispose();
    }

    private bool TryLoadCulture(string cultureName, out IReadOnlyDictionary<string, string> translations)
    {
        translations = ReadOnlyDictionary<string, string>.Empty;
        var filePath = GetLanguageFilePath(cultureName);

        try
        {
            if (!File.Exists(filePath))
            {
                Log.Warning("Localization file not found: {FilePath}", filePath);
                return false;
            }

            var json = File.ReadAllText(filePath);
            var values = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            translations = values is null
                ? ReadOnlyDictionary<string, string>.Empty
                : new ReadOnlyDictionary<string, string>(values);
            return true;
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
        {
            Log.Error(ex, "Failed to load localization file {FilePath}", filePath);
            return false;
        }
    }

    private void ResetWatcher(string cultureName)
    {
        _watcher?.Dispose();
        Directory.CreateDirectory(_languageDirectory);

        _watcher = new FileSystemWatcher(_languageDirectory, $"{cultureName}.json")
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
        };

        _watcher.Changed += (_, _) => ScheduleReload();
        _watcher.Created += (_, _) => ScheduleReload();
        _watcher.Renamed += (_, _) => ScheduleReload();
        _watcher.EnableRaisingEvents = true;
    }

    private void ScheduleReload()
    {
        _reloadTimer ??= new Timer(_ => ReloadCurrentCultureOnUiThread(), null, Timeout.Infinite, Timeout.Infinite);
        _reloadTimer.Change(ReloadDebounceMilliseconds, Timeout.Infinite);
    }

    private void ReloadCurrentCultureOnUiThread()
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is not null && !dispatcher.CheckAccess())
        {
            dispatcher.BeginInvoke(() => ApplyCulture(CurrentCulture.Name));
            return;
        }

        ApplyCulture(CurrentCulture.Name);
    }

    private string GetLanguageFilePath(string cultureName)
    {
        return Path.Combine(_languageDirectory, $"{cultureName}.json");
    }
}
