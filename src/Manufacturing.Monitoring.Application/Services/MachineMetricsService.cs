using Manufacturing.Monitoring.Application.Interfaces;
using Manufacturing.Monitoring.Domain.Enums;

namespace Manufacturing.Monitoring.Application.Services;

public sealed class MachineMetricsService : IMachineMetricsService
{
    private readonly ITelemetryRepository _repository;

    public MachineMetricsService(ITelemetryRepository repository)
    {
        _repository = repository;
    }

    public async Task<MachineStatus?> GetCurrentStatusAsync(string machineId)
    {
        var now = DateTime.UtcNow;
        var events = await _repository.GetByMachineAsync(
            machineId,
            now.AddHours(-24),
            now);

        return events.LastOrDefault()?.Status;
    }

    public async Task<(TimeSpan uptime, TimeSpan downtime, TimeSpan currentDowntime)>
        CalculateUptimeAsync(string machineId, TimeSpan window)
    {
        var now = DateTime.UtcNow;
        var from = now.Subtract(window);

        var events = await _repository.GetByMachineAsync(machineId, from, now);

        if (!events.Any())
            return (TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);

        TimeSpan uptime = TimeSpan.Zero;
        TimeSpan downtime = TimeSpan.Zero;

        var lastStatus = events.First().Status;
        var lastTimestamp = from;

        foreach (var evt in events)
        {
            var duration = evt.TimestampUtc - lastTimestamp;

            if (lastStatus == MachineStatus.Running)
                uptime += duration;
            else
                downtime += duration;

            lastStatus = evt.Status;
            lastTimestamp = evt.TimestampUtc;
        }

        var tailDuration = now - lastTimestamp;
        if (lastStatus == MachineStatus.Running)
            uptime += tailDuration;
        else
            downtime += tailDuration;

        var currentDowntime = lastStatus == MachineStatus.Stopped
            ? now - lastTimestamp
            : TimeSpan.Zero;

        return (uptime, downtime, currentDowntime);
    }
}