namespace IndustrialMonitor.Shared.Dtos;

public class DeviceDto
{
    public int Id { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string ProtocolType { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public int CollectInterval { get; set; }
    public bool IsEnabled { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
}

public class DeviceDataDto
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public DateTime CollectTime { get; set; }
    public double Temperature { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Pressure { get; set; }
    public string RunStatus { get; set; } = string.Empty;
    public bool IsAlarm { get; set; }
    public string? RawData { get; set; }
}

public class AlarmRecordDto
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DeviceCode { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string AlarmLevel { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public DateTime AlarmTime { get; set; }
    public DateTime? RecoverTime { get; set; }
    public bool IsConfirmed { get; set; }
    public string? ConfirmUser { get; set; }
    public DateTime? ConfirmTime { get; set; }
}

public class AlarmRuleDto
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DataKey { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public double ThresholdValue { get; set; }
    public string AlarmLevel { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

public class OperationLogDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string OperationContent { get; set; } = string.Empty;
    public DateTime OperationTime { get; set; }
    public string? Result { get; set; }
}
