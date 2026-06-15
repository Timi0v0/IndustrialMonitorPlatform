namespace IndustrialMonitor.Domain.Entities;

public class DeviceData
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
