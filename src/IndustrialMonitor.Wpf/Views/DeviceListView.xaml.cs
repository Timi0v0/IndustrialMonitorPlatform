using System.Windows.Controls;
using IndustrialMonitor.Wpf.ViewModels;

namespace IndustrialMonitor.Wpf.Views;

/// <summary>
/// 设备列表页面
/// </summary>
public partial class DeviceListView : UserControl
{
    public DeviceListView(DeviceListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // 页面加载时自动加载设备列表
        Loaded += async (_, _) => await viewModel.LoadDevicesCommand.ExecuteAsync(null);
    }
}
