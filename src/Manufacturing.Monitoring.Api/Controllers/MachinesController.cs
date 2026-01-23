using Manufacturing.Monitoring.Api.Dtos;
using Manufacturing.Monitoring.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Manufacturing.Monitoring.Api.Controllers;

[ApiController]
[Route("api/machines")]
public sealed class MachinesController : ControllerBase
{
    private readonly IMachineMetricsService _metrics;

    public MachinesController(IMachineMetricsService metrics)
    {
        _metrics = metrics;
    }

    [HttpGet("{machineId}/status")]
    public async Task<IActionResult> GetStatus(string machineId)
    {
        var status = await _metrics.GetCurrentStatusAsync(machineId);

        if (status is null)
            return NotFound();

        return Ok(new
        {
            MachineId = machineId,
            Status = status.ToString()
        });
    }

    [HttpGet("{machineId}/metrics")]
    public async Task<ActionResult<MachineMetricsDto>> GetMetrics(
        string machineId,
        [FromQuery] int windowMinutes = 60)
    {
        var window = TimeSpan.FromMinutes(windowMinutes);

        var status = await _metrics.GetCurrentStatusAsync(machineId)
                     ?? Domain.Enums.MachineStatus.Running;

        var (uptime, downtime, currentDowntime) =
            await _metrics.CalculateUptimeAsync(machineId, window);

        var total = uptime + downtime;
        var uptimePct = total.TotalSeconds == 0
            ? 100
            : uptime.TotalSeconds / total.TotalSeconds * 100;

        return Ok(new MachineMetricsDto(
            machineId,
            status.ToString(),
            Math.Round(uptimePct, 2),
            uptime,
            downtime,
            currentDowntime));
    }
}