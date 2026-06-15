using System.Windows;
using IndustrialMonitor.Wpf.Views;

namespace IndustrialMonitor.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(DeviceListView deviceListView)
    {
        InitializeComponent();
        RootContainer.Children.Add(deviceListView);
    }
}
