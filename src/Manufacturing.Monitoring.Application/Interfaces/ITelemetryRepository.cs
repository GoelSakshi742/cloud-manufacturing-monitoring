using Manufacturing.Monitoring.Domain.Entities;

namespace Manufacturing.Monitoring.Application.Interfaces;

/// <summary>
/// Abstraction for accessing telemetry events.
/// </summary>
public interface ITelemetryRepository
{
    Task AddAsync(TelemetryEvent telemetryEvent);

    Task<IReadOnlyList<TelemetryEvent>> GetByMachineAsync(
        string machineId,
        DateTime fromUtc,
        DateTime toUtc);
}