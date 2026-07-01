using System.Globalization;
using System.Windows.Data;

namespace IndustrialMonitor.Wpf.StartupChecks;

public sealed class StartupSeverityTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            StartupCheckSeverity.Error => "错误",
            StartupCheckSeverity.Warning => "警告",
            _ => "正常"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
