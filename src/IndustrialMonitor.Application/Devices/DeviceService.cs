using IndustrialMonitor.Application.Interfaces;
using IndustrialMonitor.Domain.Entities;
using IndustrialMonitor.Shared.Dtos;
using Mapster;

namespace IndustrialMonitor.Application.Devices;

/// <summary>
/// 设备管理业务逻辑服务
/// </summary>
public class DeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    /// <summary>
    /// 构造函数，注入设备仓储
    /// </summary>
    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    /// <summary>
    /// 获取所有设备
    /// </summary>
    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();
        return devices.Adapt<IEnumerable<DeviceDto>>();
    }

    /// <summary>
    /// 根据主键获取设备
    /// </summary>
    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        return device?.Adapt<DeviceDto>();
    }

    /// <summary>
    /// 根据设备编码获取设备
    /// </summary>
    public async Task<DeviceDto?> GetByCodeAsync(string deviceCode)
    {
        var device = await _deviceRepository.GetByCodeAsync(deviceCode);
        return device?.Adapt<DeviceDto>();
    }

    /// <summary>
    /// 获取所有已启用的设备
    /// </summary>
    public async Task<IEnumerable<DeviceDto>> GetEnabledDevicesAsync()
    {
        var devices = await _deviceRepository.GetEnabledDevicesAsync();
        return devices.Adapt<IEnumerable<DeviceDto>>();
    }

    /// <summary>
    /// 新增设备
    /// </summary>
    /// <param name="dto">设备 DTO</param>
    /// <returns>操作结果，包含错误信息（如有）</returns>
    public async Task<(DeviceDto? Device, string? Error)> CreateAsync(DeviceDto dto)
    {
        // 校验设备编码是否已存在
        if (await _deviceRepository.ExistsByCodeAsync(dto.DeviceCode))
        {
            return (null, $"设备编码 '{dto.DeviceCode}' 已存在");
        }

        var device = dto.Adapt<Device>();
        var created = await _deviceRepository.AddAsync(device);
        return (created.Adapt<DeviceDto>(), null);
    }

    /// <summary>
    /// 更新设备
    /// </summary>
    /// <param name="id">设备主键</param>
    /// <param name="dto">设备 DTO</param>
    /// <returns>操作结果，包含错误信息（如有）</returns>
    public async Task<(DeviceDto? Device, string? Error)> UpdateAsync(int id, DeviceDto dto)
    {
        // 检查设备是否存在
        var existing = await _deviceRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return (null, $"设备 ID '{id}' 不存在");
        }

        // 如果编码被修改，检查新编码是否被其他设备占用
        if (existing.DeviceCode != dto.DeviceCode
            && await _deviceRepository.ExistsByCodeAsync(dto.DeviceCode))
        {
            return (null, $"设备编码 '{dto.DeviceCode}' 已被其他设备使用");
        }

        // 将 dto 的值映射到已存在的实体上，保留 Id
        dto.Adapt(existing);
        existing.Id = id;

        await _deviceRepository.UpdateAsync(existing);
        return (existing.Adapt<DeviceDto>(), null);
    }

    /// <summary>
    /// 删除设备
    /// </summary>
    /// <returns>操作结果，包含错误信息（如有）</returns>
    public async Task<string?> DeleteAsync(int id)
    {
        var existing = await _deviceRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return $"设备 ID '{id}' 不存在";
        }

        await _deviceRepository.DeleteAsync(id);
        return null;
    }
}
