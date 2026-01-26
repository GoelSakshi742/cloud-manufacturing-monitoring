using Manufacturing.Monitoring.Api.Dtos;
using Manufacturing.Monitoring.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Manufacturing.Monitoring.Api.Controllers;

[Route("api/machines")]
public sealed class MachinesController : ControllerBase
{
    private readonly IMachineMetricsService _metrics;
    private readonly IMachineHistoryService _history;

    public MachinesController(IMachineMetricsService metrics)
    {
        _metrics = metrics;
    }
    public MachinesController(
        IMachineMetricsService metrics,
        IMachineHistoryService history)
    {
        _metrics = metrics;
        _history = history;
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
    [HttpGet("{machineId}/telemetry")]
    public async Task<ActionResult<IEnumerable<TelemetryEventDto>>> GetTelemetry(
        string machineId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc)
    {
        var events = await _history.GetTelemetryAsync(machineId, fromUtc, toUtc);

        return Ok(events.Select(e => new TelemetryEventDto(
            e.MachineId,
            e.Status.ToString(),
            e.TimestampUtc)));
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
    [HttpGet("{machineId}/timeline")]
    public async Task<ActionResult<IEnumerable<StatusTimelineDto>>> GetTimeline(
        string machineId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc)
    {
        var timeline = await _history.GetStatusTimelineAsync(machineId, fromUtc, toUtc);

        return Ok(timeline.Select(t => new StatusTimelineDto(
            t.status,
            t.from,
            t.to,
            t.to - t.from)));
    }
}