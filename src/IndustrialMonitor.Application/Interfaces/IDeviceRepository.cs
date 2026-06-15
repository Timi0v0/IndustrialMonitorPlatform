using IndustrialMonitor.Domain.Entities;

namespace IndustrialMonitor.Application.Interfaces;

/// <summary>
/// 设备仓储接口，定义设备实体的数据访问契约
/// </summary>
public interface IDeviceRepository
{
    /// <summary>根据主键获取设备</summary>
    Task<Device?> GetByIdAsync(int id);

    /// <summary>根据设备编码获取设备（编码唯一）</summary>
    Task<Device?> GetByCodeAsync(string deviceCode);

    /// <summary>获取所有设备列表</summary>
    Task<IEnumerable<Device>> GetAllAsync();

    /// <summary>获取所有已启用的设备（供采集服务使用）</summary>
    Task<IEnumerable<Device>> GetEnabledDevicesAsync();

    /// <summary>新增设备，自动填充 CreatedTime</summary>
    Task<Device> AddAsync(Device device);

    /// <summary>更新设备，自动刷新 UpdatedTime</summary>
    Task UpdateAsync(Device device);

    /// <summary>根据主键删除设备</summary>
    Task DeleteAsync(int id);

    /// <summary>检查设备编码是否已存在</summary>
    Task<bool> ExistsByCodeAsync(string deviceCode);
}

/// <summary>
/// 设备数据仓储接口，定义采集数据的数据访问契约
/// </summary>
public interface IDeviceDataRepository
{
    /// <summary>根据主键获取采集数据</summary>
    Task<DeviceData?> GetByIdAsync(int id);

    /// <summary>按设备和时间范围查询采集数据</summary>
    Task<IEnumerable<DeviceData>> GetByDeviceIdAsync(int deviceId, DateTime start, DateTime end);

    /// <summary>新增采集数据</summary>
    Task<DeviceData> AddAsync(DeviceData data);

    /// <summary>获取指定设备的最新 N 条采集数据</summary>
    Task<IEnumerable<DeviceData>> GetLatestByDeviceAsync(int deviceId, int count);
}

/// <summary>
/// 报警记录仓储接口，定义报警记录的数据访问契约
/// </summary>
public interface IAlarmRecordRepository
{
    /// <summary>根据主键获取报警记录</summary>
    Task<AlarmRecord?> GetByIdAsync(int id);

    /// <summary>获取当前所有未恢复的报警</summary>
    Task<IEnumerable<AlarmRecord>> GetActiveAlarmsAsync();

    /// <summary>按设备查询报警记录</summary>
    Task<IEnumerable<AlarmRecord>> GetByDeviceIdAsync(int deviceId);

    /// <summary>按时间范围查询报警记录</summary>
    Task<IEnumerable<AlarmRecord>> GetByTimeRangeAsync(DateTime start, DateTime end);

    /// <summary>新增报警记录</summary>
    Task<AlarmRecord> AddAsync(AlarmRecord record);

    /// <summary>更新报警记录（如确认、恢复操作）</summary>
    Task UpdateAsync(AlarmRecord record);
}

/// <summary>
/// 报警规则仓储接口，定义报警规则的数据访问契约
/// </summary>
public interface IAlarmRuleRepository
{
    /// <summary>根据主键获取报警规则</summary>
    Task<AlarmRule?> GetByIdAsync(int id);

    /// <summary>按设备查询报警规则</summary>
    Task<IEnumerable<AlarmRule>> GetByDeviceIdAsync(int deviceId);

    /// <summary>获取所有已启用的报警规则</summary>
    Task<IEnumerable<AlarmRule>> GetEnabledRulesAsync();

    /// <summary>新增报警规则</summary>
    Task<AlarmRule> AddAsync(AlarmRule rule);

    /// <summary>更新报警规则</summary>
    Task UpdateAsync(AlarmRule rule);

    /// <summary>删除报警规则</summary>
    Task DeleteAsync(int id);
}

/// <summary>
/// 操作日志仓储接口，定义操作日志的数据访问契约
/// </summary>
public interface IOperationLogRepository
{
    /// <summary>根据主键获取操作日志</summary>
    Task<OperationLog?> GetByIdAsync(int id);

    /// <summary>按时间范围查询操作日志</summary>
    Task<IEnumerable<OperationLog>> GetByTimeRangeAsync(DateTime start, DateTime end);

    /// <summary>新增操作日志</summary>
    Task<OperationLog> AddAsync(OperationLog log);

    /// <summary>获取最近 N 条操作日志</summary>
    Task<IEnumerable<OperationLog>> GetRecentAsync(int count);
}
