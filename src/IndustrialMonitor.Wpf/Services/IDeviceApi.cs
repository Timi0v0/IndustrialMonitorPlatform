using IndustrialMonitor.Shared.Dtos;
using Refit;

namespace IndustrialMonitor.Wpf.Services;

/// <summary>
/// 设备管理 API 接口（Refit），与后端 DevicesController 对应
/// </summary>
public interface IDeviceApi
{
    /// <summary>获取所有设备</summary>
    [Get("/api/devices")]
    Task<List<DeviceDto>> GetAllAsync();

    /// <summary>根据 ID 获取设备</summary>
    [Get("/api/devices/{id}")]
    Task<DeviceDto> GetByIdAsync(int id);

    /// <summary>根据编码获取设备</summary>
    [Get("/api/devices/code/{code}")]
    Task<DeviceDto> GetByCodeAsync(string code);

    /// <summary>获取已启用设备</summary>
    [Get("/api/devices/enabled")]
    Task<List<DeviceDto>> GetEnabledAsync();

    /// <summary>新增设备</summary>
    [Post("/api/devices")]
    Task<DeviceDto> CreateAsync([Body] DeviceDto dto);

    /// <summary>更新设备</summary>
    [Put("/api/devices/{id}")]
    Task<DeviceDto> UpdateAsync(int id, [Body] DeviceDto dto);

    /// <summary>删除设备</summary>
    [Delete("/api/devices/{id}")]
    Task DeleteAsync(int id);
}
