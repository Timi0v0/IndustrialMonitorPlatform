using IndustrialMonitor.DeviceSimulator.Configs;
using IndustrialMonitor.Shared.CommonModels;

namespace IndustrialMonitor.DeviceSimulator.Simulators
{
    /// <summary>
    /// 正常数据模拟器：生成工业设备正常运行范围内的数据
    /// </summary>
    public class NormalDataSimulator : DeviceSimulatorBase
    {
        private static readonly Random Rng = new();

        public NormalDataSimulator(DeviceConfig config) : base(config)
        {
        }

        protected override DeviceDataPayload GenerateData()
        {
            return new DeviceDataPayload
            {
                DeviceCode = Config.DeviceCode,
                CollectTime = DateTime.Now,
                // 工业设备正常运行范围
                Temperature = 20 + Rng.NextDouble() * 30,      // 20~50℃
                Voltage = 10 + Rng.NextDouble() * 4,           // 10~14V
                Current = 3 + Rng.NextDouble() * 5,            // 3~8A
                Pressure = 0.3 + Rng.NextDouble() * 0.5,       // 0.3~0.8MPa
                RunStatus = "Running",
                IsAlarm = false
            };
        }
    }
}
