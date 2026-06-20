using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DeviceSimulator.Configs
{
    /// <summary>
    /// 配置类 
    /// </summary>
    internal class SimulatorRootConfig
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; set; } = 5000;

        /// <summary>
        /// 设备列表，每个设备包含设备编码、数据发送间隔、是否混入异常数据等配置
        /// </summary>
        public List<DeviceConfig> Devices { get; set; } = new();


    }

    public class DeviceConfig
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; } = "Device001";

        //设备数据发送间隔（毫秒）
        public int SendIntervalMs { get; set; } = 1000;

        //是否混入异常数据
        public bool EnableAbnormal { get; set; } = false;

        //异常数据比例（0-1之间）
        public double AbnormalProbability { get; set; } = 0.1;


    }

}
