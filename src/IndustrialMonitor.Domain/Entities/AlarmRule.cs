namespace IndustrialMonitor.Domain.Entities;

public class AlarmRule
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
