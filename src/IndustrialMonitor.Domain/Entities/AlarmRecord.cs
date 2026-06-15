namespace IndustrialMonitor.Domain.Entities;

public class AlarmRecord
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
