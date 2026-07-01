using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class StartupSeverityBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            StartupCheckSeverity.Error => new SolidColorBrush(Color.FromRgb(220, 38, 38)),
            StartupCheckSeverity.Warning => new SolidColorBrush(Color.FromRgb(217, 119, 6)),
            _ => new SolidColorBrush(Color.FromRgb(37, 99, 235))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
