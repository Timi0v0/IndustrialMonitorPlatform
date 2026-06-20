using IndustrialMonitor.DeviceSimulator.Configs;
using IndustrialMonitor.Shared.CommonModels;

namespace IndustrialMonitor.DeviceSimulator.Simulators
{
    /// <summary>
    /// 异常数据模拟器：按概率混入超限数据，并可模拟随机断线
    /// </summary>
    public class AbnormalDataSimulator : DeviceSimulatorBase
    {
        private static readonly Random Rng = new();

        public AbnormalDataSimulator(DeviceConfig config) : base(config)
        {
        }

        protected override DeviceDataPayload GenerateData()
        {
            // 按概率决定本次是否出异常
            if (Rng.NextDouble() < Config.AbnormalProbability)
            {
                return GenerateAbnormalData();
            }
            return GenerateNormalData();
        }

        private DeviceDataPayload GenerateNormalData()
        {
            return new DeviceDataPayload
            {
                DeviceCode = Config.DeviceCode,
                CollectTime = DateTime.Now,
                Temperature = 20 + Rng.NextDouble() * 30,      // 20~50℃
                Voltage = 10 + Rng.NextDouble() * 4,           // 10~14V
                Current = 3 + Rng.NextDouble() * 5,            // 3~8A
                Pressure = 0.3 + Rng.NextDouble() * 0.5,       // 0.3~0.8MPa
                RunStatus = "Running",
                IsAlarm = false
            };
        }

        // 随机选一种异常类型
        private DeviceDataPayload GenerateAbnormalData()
        {
            return new DeviceDataPayload
            {
                DeviceCode = Config.DeviceCode,
                CollectTime = DateTime.Now,
                Temperature = Rng.Next(0, 2) == 0
                    ? 65 + Rng.NextDouble() * 20   // 65~85℃ 高温
                    : 20 + Rng.NextDouble() * 30,   // 正常范围
                Voltage = 15 + Rng.NextDouble() * 5,    // 15~20V 过压
                Current = 20 + Rng.NextDouble() * 10,   // 20~30A 过流
                Pressure = 0.3 + Rng.NextDouble() * 0.5,
                RunStatus = "Fault",
                IsAlarm = true
            };
        }

        // 随机断线：5% 概率断开
        protected override bool ShouldDisconnect() => Rng.NextDouble() < 0.05;
    }
}
