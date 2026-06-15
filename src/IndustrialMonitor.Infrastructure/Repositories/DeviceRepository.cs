using IndustrialMonitor.Application.Interfaces;
using IndustrialMonitor.Domain.Entities;
using IndustrialMonitor.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMonitor.Infrastructure.Repositories;

/// <summary>
/// 设备仓储实现，提供设备实体的数据库 CRUD 操作
/// </summary>
public class DeviceRepository : IDeviceRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 构造函数，注入数据库上下文
    /// </summary>
    /// <param name="context">应用数据库上下文</param>
    public DeviceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Device?> GetByIdAsync(int id)
    {
        // 使用 FindAsync 按主键查询，EF Core 会自动检查本地缓存
        return await _context.Devices.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<Device?> GetByCodeAsync(string deviceCode)
    {
        // 使用设备编码查询，DeviceCode 字段有唯一索引约束
        return await _context.Devices.FirstOrDefaultAsync(d => d.DeviceCode == deviceCode);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        // 查询所有设备，按设备编码排序输出
        return await _context.Devices.OrderBy(d => d.DeviceCode).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Device>> GetEnabledDevicesAsync()
    {
        // 仅查询已启用的设备，供采集服务使用
        return await _context.Devices
            .Where(d => d.IsEnabled)
            .OrderBy(d => d.DeviceCode)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Device> AddAsync(Device device)
    {
        // 新增时自动设置创建时间（UTC，避免时区问题）
        device.CreatedTime = DateTime.UtcNow;
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Device device)
    {
        // 更新时自动刷新修改时间
        device.UpdatedTime = DateTime.UtcNow;
        _context.Devices.Update(device);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        // 先查找设备，存在则删除；不存在时静默忽略
        var device = await _context.Devices.FindAsync(id);
        if (device != null)
        {
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByCodeAsync(string deviceCode)
    {
        // 检查设备编码是否已被占用，用于新增/更新时的唯一性校验
        return await _context.Devices.AnyAsync(d => d.DeviceCode == deviceCode);
    }
}
