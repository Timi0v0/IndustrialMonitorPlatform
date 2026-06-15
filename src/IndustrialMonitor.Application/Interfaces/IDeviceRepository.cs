using IndustrialMonitor.Domain.Entities;

namespace IndustrialMonitor.Application.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(int id);
    Task<Device?> GetByCodeAsync(string deviceCode);
    Task<IEnumerable<Device>> GetAllAsync();
    Task<IEnumerable<Device>> GetEnabledDevicesAsync();
    Task<Device> AddAsync(Device device);
    Task UpdateAsync(Device device);
    Task DeleteAsync(int id);
    Task<bool> ExistsByCodeAsync(string deviceCode);
}

public interface IDeviceDataRepository
{
    Task<DeviceData?> GetByIdAsync(int id);
    Task<IEnumerable<DeviceData>> GetByDeviceIdAsync(int deviceId, DateTime start, DateTime end);
    Task<DeviceData> AddAsync(DeviceData data);
    Task<IEnumerable<DeviceData>> GetLatestByDeviceAsync(int deviceId, int count);
}

public interface IAlarmRecordRepository
{
    Task<AlarmRecord?> GetByIdAsync(int id);
    Task<IEnumerable<AlarmRecord>> GetActiveAlarmsAsync();
    Task<IEnumerable<AlarmRecord>> GetByDeviceIdAsync(int deviceId);
    Task<IEnumerable<AlarmRecord>> GetByTimeRangeAsync(DateTime start, DateTime end);
    Task<AlarmRecord> AddAsync(AlarmRecord record);
    Task UpdateAsync(AlarmRecord record);
}

public interface IAlarmRuleRepository
{
    Task<AlarmRule?> GetByIdAsync(int id);
    Task<IEnumerable<AlarmRule>> GetByDeviceIdAsync(int deviceId);
    Task<IEnumerable<AlarmRule>> GetEnabledRulesAsync();
    Task<AlarmRule> AddAsync(AlarmRule rule);
    Task UpdateAsync(AlarmRule rule);
    Task DeleteAsync(int id);
}

public interface IOperationLogRepository
{
    Task<OperationLog?> GetByIdAsync(int id);
    Task<IEnumerable<OperationLog>> GetByTimeRangeAsync(DateTime start, DateTime end);
    Task<OperationLog> AddAsync(OperationLog log);
    Task<IEnumerable<OperationLog>> GetRecentAsync(int count);
}
