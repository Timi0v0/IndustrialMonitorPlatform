using IndustrialMonitor.Application.Devices;
using IndustrialMonitor.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace IndustrialMonitor.Api.Controllers;

/// <summary>
/// 设备管理控制器，提供设备的增删改查 RESTful API
/// </summary>
[ApiController]
[Route("api/devices")]
public class DevicesController : ControllerBase
{
    private readonly DeviceService _deviceService;

    /// <summary>
    /// 构造函数，注入设备管理服务
    /// </summary>
    public DevicesController(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    /// <summary>
    /// 获取所有设备列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAll()
    {
        var devices = await _deviceService.GetAllAsync();
        return Ok(devices);
    }

    /// <summary>
    /// 根据主键获取设备详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceDto>> GetById(int id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        if (device == null)
        {
            return NotFound(new { message = $"设备 ID '{id}' 不存在" });
        }
        return Ok(device);
    }

    /// <summary>
    /// 根据设备编码获取设备
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<DeviceDto>> GetByCode(string code)
    {
        var device = await _deviceService.GetByCodeAsync(code);
        if (device == null)
        {
            return NotFound(new { message = $"设备编码 '{code}' 不存在" });
        }
        return Ok(device);
    }

    /// <summary>
    /// 获取所有已启用的设备
    /// </summary>
    [HttpGet("enabled")]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetEnabled()
    {
        var devices = await _deviceService.GetEnabledDevicesAsync();
        return Ok(devices);
    }

    /// <summary>
    /// 新增设备
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DeviceDto>> Create([FromBody] DeviceDto dto)
    {
        var (device, error) = await _deviceService.CreateAsync(dto);
        if (error != null)
        {
            return Conflict(new { message = error });
        }
        return CreatedAtAction(nameof(GetById), new { id = device!.Id }, device);
    }

    /// <summary>
    /// 更新设备
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<DeviceDto>> Update(int id, [FromBody] DeviceDto dto)
    {
        var (device, error) = await _deviceService.UpdateAsync(id, dto);
        if (error != null)
        {
            // 区分"不存在"和"编码冲突"
            var existing = await _deviceService.GetByIdAsync(id);
            return existing == null
                ? NotFound(new { message = error })
                : Conflict(new { message = error });
        }
        return Ok(device);
    }

    /// <summary>
    /// 删除设备
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var error = await _deviceService.DeleteAsync(id);
        if (error != null)
        {
            return NotFound(new { message = error });
        }
        return NoContent();
    }
}
