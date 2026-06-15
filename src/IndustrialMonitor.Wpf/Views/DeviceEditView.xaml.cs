using System.Windows.Controls;
using IndustrialMonitor.Wpf.ViewModels;

namespace IndustrialMonitor.Wpf.Views;

/// <summary>
/// 设备编辑对话框
/// </summary>
public partial class DeviceEditView : UserControl
{
    public DeviceEditView(DeviceEditViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
