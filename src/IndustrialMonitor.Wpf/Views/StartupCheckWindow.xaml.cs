using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using IndustrialMonitor.Wpf.StartupChecks;

namespace IndustrialMonitor.Wpf.Views;

public partial class StartupCheckWindow : Window
{
    private readonly IReadOnlyList<StartupCheckResult> _results;

    public StartupCheckWindow(IReadOnlyList<StartupCheckResult> results, bool allowContinue)
    {
        _results = results;

        InitializeComponent();
        DataContext = new StartupCheckWindowModel(results, allowContinue);
    }

    public bool RetryRequested { get; private set; }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(BuildDiagnosticsText());
    }

    private void RetryButton_Click(object sender, RoutedEventArgs e)
    {
        RetryRequested = true;
        DialogResult = false;
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private string BuildDiagnosticsText()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Industrial Monitor startup diagnostics");
        builder.AppendLine($"Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"BaseDirectory: {AppContext.BaseDirectory}");
        builder.AppendLine();

        foreach (var result in _results)
        {
            builder.AppendLine($"[{result.Severity}] {result.Key}");
            builder.AppendLine(result.Title);
            builder.AppendLine(result.Message);
            if (!string.IsNullOrWhiteSpace(result.Remedy))
            {
                builder.AppendLine(result.Remedy);
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private sealed class StartupCheckWindowModel
    {
        public StartupCheckWindowModel(IReadOnlyList<StartupCheckResult> results, bool allowContinue)
        {
            Results = new ObservableCollection<StartupCheckResult>(results);
            AllowContinue = allowContinue;
            Title = allowContinue ? "启动环境存在警告" : "启动环境检查失败";
            Description = allowContinue
                ? "检测到一些非致命问题，可以继续进入系统，也可以修复后重试。"
                : "检测到致命问题，软件暂时无法进入主界面。请根据修复建议处理后重试。";
        }

        public string Title { get; }

        public string Description { get; }

        public bool AllowContinue { get; }

        public ObservableCollection<StartupCheckResult> Results { get; }
    }
}
