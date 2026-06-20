using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Shared.CommonModels
{
    /// <summary>
    /// 设备负载数据
    /// </summary>
    public class DeviceDataPayload
    {
        public string DeviceCode { get; set; } = "";
        public DateTime CollectTime { get; set; }
        public double Temperature { get; set; }
        public double Voltage { get; set; }
        public double Current { get; set; }
        public double Pressure { get; set; }
        public string RunStatus { get; set; } = "Running";  // Running / Offline / Fault
        public bool IsAlarm { get; set; }
    }
}
