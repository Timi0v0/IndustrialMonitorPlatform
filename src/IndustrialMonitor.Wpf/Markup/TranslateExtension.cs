using System.Windows.Data;
using System.Windows.Markup;
using IndustrialMonitor.Wpf.Services;

namespace IndustrialMonitor.Wpf.Markup;

[MarkupExtensionReturnType(typeof(string))]
public class TranslateExtension : MarkupExtension
{
    public TranslateExtension()
    {
    }

    public TranslateExtension(string key)
    {
        Key = key;
    }

    [ConstructorArgument("key")]
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding($"[{Key}]")
        {
            Source = LocalizationProvider.Store,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }
}
