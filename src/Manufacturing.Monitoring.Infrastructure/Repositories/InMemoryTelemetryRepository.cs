using Manufacturing.Monitoring.Application.Interfaces;
using Manufacturing.Monitoring.Domain.Entities;
using System.Collections.Concurrent;

namespace Manufacturing.Monitoring.Infrastructure.Repositories;

/// <summary>
/// In‑memory implementation of telemetry storage.
/// Intended for simulation and local development.
/// </summary>
public sealed class InMemoryTelemetryRepository : ITelemetryRepository
{
    private readonly ConcurrentDictionary<string, List<TelemetryEvent>> _store = new();

    public Task AddAsync(TelemetryEvent telemetryEvent)
    {
        var events = _store.GetOrAdd(
            telemetryEvent.MachineId,
            _ => new List<TelemetryEvent>());

        lock (events)
        {
            events.Add(telemetryEvent);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TelemetryEvent>> GetByMachineAsync(
        string machineId,
        DateTime fromUtc,
        DateTime toUtc)
    {
        if (!_store.TryGetValue(machineId, out var events))
            return Task.FromResult<IReadOnlyList<TelemetryEvent>>(Array.Empty<TelemetryEvent>());

        List<TelemetryEvent> result;

        lock (events)
        {
            result = events
                .Where(e => e.TimestampUtc >= fromUtc && e.TimestampUtc <= toUtc)
                .OrderBy(e => e.TimestampUtc)
                .ToList();
        }

        return Task.FromResult<IReadOnlyList<TelemetryEvent>>(result);
    }
}