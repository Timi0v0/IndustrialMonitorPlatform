using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using IndustrialMonitor.Wpf.Services;

namespace IndustrialMonitor.Wpf.Markup;

[MarkupExtensionReturnType(typeof(string))]
public class TranslateFormatExtension : MarkupExtension
{
    public TranslateFormatExtension()
    {
    }

    public TranslateFormatExtension(string key)
    {
        Key = key;
    }

    [ConstructorArgument("key")]
    public string Key { get; set; } = string.Empty;

    public BindingBase? Value { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new MultiBinding
        {
            Converter = new LocalizationFormatConverter(Key),
            Mode = BindingMode.OneWay
        };

        binding.Bindings.Add(new Binding(nameof(LocalizationStore.Version))
        {
            Source = LocalizationProvider.Store,
            Mode = BindingMode.OneWay
        });

        if (Value is not null)
        {
            binding.Bindings.Add(Value);
        }

        return binding.ProvideValue(serviceProvider);
    }

    private sealed class LocalizationFormatConverter : IMultiValueConverter
    {
        private readonly string _key;

        public LocalizationFormatConverter(string key)
        {
            _key = key;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var args = values.Skip(1)
                .Where(value => !ReferenceEquals(value, DependencyProperty.UnsetValue))
                .ToArray();
            return LocalizationProvider.Store.Format(_key, args);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
