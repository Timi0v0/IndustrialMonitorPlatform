using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using IndustrialMonitor.Shared.Dtos;
using IndustrialMonitor.Wpf.Services;
using IndustrialMonitor.Wpf.Views;
using Serilog;

namespace IndustrialMonitor.Wpf.ViewModels;

/// <summary>
/// 设备列表页 ViewModel
/// </summary>
public partial class DeviceListViewModel : ObservableObject
{
    private readonly IDeviceApi _deviceApi;

    /// <summary>设备列表</summary>
    public ObservableCollection<DeviceDto> Devices { get; } = [];

    /// <summary>搜索关键字</summary>
    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>是否正在加载</summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>总设备数</summary>
    [ObservableProperty]
    private int _totalCount;

    /// <summary>选中的设备</summary>
    [ObservableProperty]
    private DeviceDto? _selectedDevice;

    public DeviceListViewModel(IDeviceApi deviceApi)
    {
        _deviceApi = deviceApi;
    }

    /// <summary>加载设备列表</summary>
    [RelayCommand]
    private async Task LoadDevicesAsync()
    {
        try
        {
            IsLoading = true;
            var devices = await _deviceApi.GetAllAsync();
            Devices.Clear();
            foreach (var device in devices)
            {
                Devices.Add(device);
            }
            TotalCount = Devices.Count;
            Log.Information("已加载 {Count} 台设备", TotalCount);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "加载设备列表失败");
            Growl.Error("加载设备列表失败，请确认后端服务是否启动");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>搜索设备</summary>
    [RelayCommand]
    private void SearchDevices()
    {
        // 后续实现本地过滤或调用后端搜索接口
    }

    /// <summary>刷新列表</summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDevicesAsync();
    }

    /// <summary>新增设备</summary>
    [RelayCommand]
    private async Task AddDeviceAsync()
    {
        var editVm = new DeviceEditViewModel();
        var editView = new DeviceEditView(editVm);
        var dialog = Dialog.Show(editView);

        // 当保存按钮点击时, 提交数据并关闭对话框
        editVm.OnSaveRequested = async (dto) =>
        {
            try
            {
                var created = await _deviceApi.CreateAsync(dto);
                Devices.Add(created);
                TotalCount = Devices.Count;
                dialog.Close();
                Growl.Success($"设备 {created.DeviceCode} 创建成功");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "创建设备失败");
                Growl.Error("创建设备失败，请检查输入");
            }
        };
    }

    /// <summary>编辑设备</summary>
    [RelayCommand]
    private async Task EditDeviceAsync(DeviceDto? device)
    {
        if (device == null) return;

        var editVm = new DeviceEditViewModel();
        editVm.LoadFromDto(device);
        var editView = new DeviceEditView(editVm);
        var dialog = Dialog.Show(editView);

        editVm.OnSaveRequested = async (dto) =>
        {
            try
            {
                var updated = await _deviceApi.UpdateAsync(device.Id, dto);
                var index = Devices.IndexOf(device);
                if (index >= 0)
                {
                    Devices[index] = updated;
                }
                dialog.Close();
                Growl.Success($"设备 {updated.DeviceCode} 更新成功");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "更新设备失败");
                Growl.Error("更新设备失败，请检查输入");
            }
        };
    }

    /// <summary>删除设备</summary>
    [RelayCommand]
    private async Task DeleteDeviceAsync(DeviceDto? device)
    {
        if (device == null) return;

        // 确认对话框
        var result = HandyControl.Controls.MessageBox.Show(
            $"确定要删除设备 \"{device.DeviceName}({device.DeviceCode})\" 吗？",
            "确认删除",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _deviceApi.DeleteAsync(device.Id);
            Devices.Remove(device);
            TotalCount = Devices.Count;
            Growl.Success($"设备 {device.DeviceCode} 已删除");
            Log.Information("已删除设备 {DeviceCode}", device.DeviceCode);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除设备 {DeviceCode} 失败", device?.DeviceCode);
            Growl.Error("删除设备失败");
        }
    }
}
