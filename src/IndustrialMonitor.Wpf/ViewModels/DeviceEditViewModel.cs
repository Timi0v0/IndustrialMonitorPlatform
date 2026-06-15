using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IndustrialMonitor.Shared.Dtos;

namespace IndustrialMonitor.Wpf.ViewModels;

/// <summary>
/// 设备编辑对话框 ViewModel（新增/编辑复用）
/// </summary>
public partial class DeviceEditViewModel : ObservableObject
{
    /// <summary>编辑模式：Create 或 Edit</summary>
    public enum EditMode { Create, Edit }

    /// <summary>保存回调，由父 ViewModel 注入</summary>
    public Func<DeviceDto, Task>? OnSaveRequested { get; set; }

    [ObservableProperty]
    private EditMode _mode = EditMode.Create;

    [ObservableProperty]
    private string _windowTitle = "新增设备";

    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _deviceCode = string.Empty;

    [ObservableProperty]
    private string _deviceName = string.Empty;

    [ObservableProperty]
    private string _deviceType = string.Empty;

    [ObservableProperty]
    private string _protocolType = "TCP";

    [ObservableProperty]
    private string _ipAddress = string.Empty;

    [ObservableProperty]
    private int _port = 502;

    [ObservableProperty]
    private int _collectInterval = 5000;

    [ObservableProperty]
    private bool _isEnabled = true;

    [ObservableProperty]
    private string _remark = string.Empty;

    /// <summary>
    /// 从 DeviceDto 加载数据（编辑模式）
    /// </summary>
    public void LoadFromDto(DeviceDto dto)
    {
        Mode = EditMode.Edit;
        WindowTitle = "编辑设备";
        Id = dto.Id;
        DeviceCode = dto.DeviceCode;
        DeviceName = dto.DeviceName;
        DeviceType = dto.DeviceType;
        ProtocolType = dto.ProtocolType;
        IpAddress = dto.IpAddress;
        Port = dto.Port;
        CollectInterval = dto.CollectInterval;
        IsEnabled = dto.IsEnabled;
        Remark = dto.Remark ?? string.Empty;
    }

    /// <summary>
    /// 转换为 DTO 用于提交
    /// </summary>
    public DeviceDto ToDto()
    {
        return new DeviceDto
        {
            Id = Id,
            DeviceCode = DeviceCode,
            DeviceName = DeviceName,
            DeviceType = DeviceType,
            ProtocolType = ProtocolType,
            IpAddress = IpAddress,
            Port = Port,
            CollectInterval = CollectInterval,
            IsEnabled = IsEnabled,
            Remark = string.IsNullOrWhiteSpace(Remark) ? null : Remark
        };
    }

    /// <summary>保存命令</summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (OnSaveRequested != null)
        {
            await OnSaveRequested.Invoke(ToDto());
        }
    }
}
