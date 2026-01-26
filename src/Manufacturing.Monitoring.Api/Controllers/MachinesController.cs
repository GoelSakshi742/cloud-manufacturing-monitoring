using Manufacturing.Monitoring.Api.Dtos;
using Manufacturing.Monitoring.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Manufacturing.Monitoring.Api.Controllers;

[ApiController]
[Route("api/machines")]
public sealed class MachinesController : ControllerBase
{
    private readonly IMachineMetricsService _metrics;
    private readonly IMachineHistoryService _history;

    public MachinesController(
        IMachineMetricsService metrics,
        IMachineHistoryService history)
    {
        _metrics = metrics;
        _history = history;
    }

    // GET /api/machines/{machineId}/status
    [HttpGet("{machineId}/status")]
    public async Task<ActionResult<MachineStatusDto>> GetStatus(string machineId)
    {
        var status = await _metrics.GetCurrentStatusAsync(machineId);

        if (status is null)
            return NotFound();

        return Ok(new MachineStatusDto(
            machineId,
            status.ToString(),
            DateTime.UtcNow));
    }

    // GET /api/machines/{machineId}/telemetry
    [HttpGet("{machineId}/telemetry")]
    public async Task<ActionResult<IEnumerable<TelemetryEventDto>>> GetTelemetry(
        string machineId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc)
    {
        if (fromUtc == default || toUtc == default || fromUtc >= toUtc)
            return BadRequest("Invalid time range.");

        var events = await _history.GetTelemetryAsync(machineId, fromUtc, toUtc);

        return Ok(events.Select(e => new TelemetryEventDto(
            e.MachineId,
            e.Status.ToString(),
            e.TimestampUtc)));
    }

    // GET /api/machines/{machineId}/metrics
    [HttpGet("{machineId}/metrics")]
    public async Task<ActionResult<MachineMetricsDto>> GetMetrics(
        string machineId,
        [FromQuery] int windowMinutes = 60)
    {
        if (windowMinutes <= 0)
            return BadRequest("windowMinutes must be greater than zero.");

        var window = TimeSpan.FromMinutes(windowMinutes);

        var status = await _metrics.GetCurrentStatusAsync(machineId);

        if (status is null)
            return NotFound();

        var (uptime, downtime, currentDowntime) =
            await _metrics.CalculateUptimeAsync(machineId, window);

        var total = uptime + downtime;

        var uptimePct = total.TotalSeconds == 0
            ? 0
            : uptime.TotalSeconds / total.TotalSeconds * 100;

        return Ok(new MachineMetricsDto(
            machineId,
            status.ToString(),
            Math.Round(uptimePct, 2),
            uptime,
            downtime,
            currentDowntime));
    }

    // GET /api/machines/{machineId}/timeline
    [HttpGet("{machineId}/timeline")]
    public async Task<ActionResult<IEnumerable<StatusTimelineDto>>> GetTimeline(
        string machineId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc)
    {
        if (fromUtc == default || toUtc == default || fromUtc >= toUtc)
            return BadRequest("Invalid time range.");

        var timeline = await _history
            .GetStatusTimelineAsync(machineId, fromUtc, toUtc);

        return Ok(timeline.Select(t => new StatusTimelineDto(
            t.status,
            t.from,
            t.to,
            t.to - t.from)));
    }
}