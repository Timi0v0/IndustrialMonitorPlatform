using IndustrialMonitor.DeviceSimulator.Configs;
using IndustrialMonitor.Shared.CommonModels;
using Serilog;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace IndustrialMonitor.DeviceSimulator.Simulators
{
    public abstract class DeviceSimulatorBase : IDisposable
    {
        protected readonly DeviceConfig Config;
        private TcpClient? _client;
        private NetworkStream? _stream;
        private bool _disposed;

        protected DeviceSimulatorBase(DeviceConfig config)
        {
            Config = config;
        }

        /// <summary>
        /// 子类实现：生成该设备的模拟数据
        /// </summary>
        protected abstract DeviceDataPayload GenerateData();

        /// <summary>
        /// 启动连接 + 发送循环（含自动重连）
        /// </summary>
        public async Task StartAsync(string address, int port, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync(address, port, token);
                    _stream = _client.GetStream();

                    Log.Information("[{Device}] 已连接到 {Address}:{Port}",
                        Config.DeviceCode, address, port);

                    await SendLoopAsync(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "[{Device}] 连接断开，5秒后重连...", Config.DeviceCode);
                    CleanupConnection();
                    await Task.Delay(5000, token);
                }
            }
        }

        /// <summary>
        /// 发送循环：GenerateData → 序列化 → 发送 → 等待间隔
        /// </summary>
        private async Task SendLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _stream != null)
            {
                // 子类可控制随机断线
                if (ShouldDisconnect())
                {
                    Log.Warning("[{Device}] 模拟断线", Config.DeviceCode);
                    break;
                }

                var payload = GenerateData();
                var json = JsonSerializer.Serialize(payload);
                var data = Encoding.UTF8.GetBytes(json);

                // 4 字节长度头（大端序）
                var lenBytes = BitConverter.GetBytes(data.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBytes);

                await _stream.WriteAsync(lenBytes, token);
                await _stream.WriteAsync(data, token);
                await _stream.FlushAsync(token);

                await Task.Delay(Config.SendIntervalMs, token);
            }
        }

        /// <summary>
        /// 子类重写以控制随机断线
        /// </summary>
        protected virtual bool ShouldDisconnect() => false;

        private void CleanupConnection()
        {
            _stream?.Dispose();
            _client?.Dispose();
            _stream = null;
            _client = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            CleanupConnection();
            GC.SuppressFinalize(this);
        }
    }
}
