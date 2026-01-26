using Manufacturing.Monitoring.Application.Interfaces;
using Manufacturing.Monitoring.Domain.Entities;

namespace Manufacturing.Monitoring.Tests.Fakes;

public sealed class FakeTelemetryRepository : ITelemetryRepository
{
    private readonly List<TelemetryEvent> _events = new();

    public Task AddAsync(TelemetryEvent telemetryEvent)
    {
        _events.Add(telemetryEvent);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TelemetryEvent>> GetByMachineAsync(
        string machineId,
        DateTime fromUtc,
        DateTime toUtc)
    {
        var result = _events
            .Where(e => e.MachineId == machineId &&
                        e.TimestampUtc >= fromUtc &&
                        e.TimestampUtc <= toUtc)
            .OrderBy(e => e.TimestampUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<TelemetryEvent>>(result);
    }
}