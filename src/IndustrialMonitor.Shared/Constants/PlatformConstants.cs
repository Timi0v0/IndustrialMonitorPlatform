namespace IndustrialMonitor.Shared.Constants;

public static class PlatformConstants
{
    public const string ApplicationName = "工业设备数据采集与监控平台";
    public const string Version = "1.0.0";

    // SignalR Hub
    public const string MonitorHubPath = "/hubs/monitor";

    // SignalR Methods
    public const string ReceiveDeviceData = "ReceiveDeviceData";
    public const string ReceiveAlarm = "ReceiveAlarm";
    public const string ReceiveDeviceStatus = "ReceiveDeviceStatus";

    // Alarm Levels
    public const string AlarmLevelInfo = "Info";
    public const string AlarmLevelWarning = "Warning";
    public const string AlarmLevelCritical = "Critical";

    // Protocol Types
    public const string ProtocolTcp = "TCP";
    public const string ProtocolModbus = "ModbusTCP";
    public const string ProtocolMqtt = "MQTT";

    // Device Status
    public const string StatusOnline = "Online";
    public const string StatusOffline = "Offline";
    public const string StatusAlarm = "Alarm";

    // Cache Keys
    public const string CacheKeyDevices = "Devices";
    public const string CacheKeyDeviceData = "DeviceData";
    public const string CacheKeyAlarms = "Alarms";
}
