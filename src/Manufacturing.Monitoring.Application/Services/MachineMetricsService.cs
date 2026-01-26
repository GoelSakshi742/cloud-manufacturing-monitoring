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
        var events = await _repository.GetByMachineAsync(
            machineId,
            DateTime.MinValue,
            DateTime.UtcNow);

        return events.LastOrDefault()?.Status;
    }

    public async Task<(TimeSpan uptime, TimeSpan downtime, TimeSpan currentDowntime)>
        CalculateUptimeAsync(string machineId, TimeSpan window)
    {
        var now = DateTime.UtcNow;
        var from = now.Subtract(window);

        var events = await _repository.GetByMachineAsync(machineId, from, now);

        if (events.Count == 0)
            return (TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);

        TimeSpan uptime = TimeSpan.Zero;
        TimeSpan downtime = TimeSpan.Zero;

        var lastStatus = events[0].Status;
        var lastTimestamp = events[0].TimestampUtc;

        foreach (var evt in events.Skip(1))
        {
            var duration = evt.TimestampUtc - lastTimestamp;

            if (lastStatus == MachineStatus.Running)
                uptime += duration;
            else
                downtime += duration;

            lastStatus = evt.Status;
            lastTimestamp = evt.TimestampUtc;
        }

        // Extend last known state to now
        var tailDuration = now - lastTimestamp;

        if (lastStatus == MachineStatus.Running)
            uptime += tailDuration;
        else
            downtime += tailDuration;

        var currentDowntime =
            lastStatus == MachineStatus.Stopped
                ? tailDuration
                : TimeSpan.Zero;

        return (uptime, downtime, currentDowntime);
    }
}