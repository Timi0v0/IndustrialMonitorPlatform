using Microsoft.Extensions.Configuration;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serverAddress = configuration["ServerAddress"];
var serverPort = configuration.GetValue<int>("ServerPort");
var deviceCode = configuration["DeviceCode"];
var sendIntervalMs = configuration.GetValue<int>("SendIntervalMs");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/simulator-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Device simulator starting for {DeviceCode}...", deviceCode);
    Log.Information("Target server: {ServerAddress}:{ServerPort}", serverAddress, serverPort);
    Log.Information("Send interval: {SendIntervalMs}ms", sendIntervalMs);
    Log.Information("Simulator is running. Press Ctrl+C to exit.");
    await Task.Delay(Timeout.Infinite);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Simulator terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}
