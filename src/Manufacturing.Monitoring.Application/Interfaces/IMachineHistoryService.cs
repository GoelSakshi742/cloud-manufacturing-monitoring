using Manufacturing.Monitoring.Domain.Entities;

namespace Manufacturing.Monitoring.Application.Interfaces;

public interface IMachineHistoryService
{
    Task<IReadOnlyList<TelemetryEvent>> GetTelemetryAsync(
        string machineId,
        DateTime fromUtc,
        DateTime toUtc);

    Task<IReadOnlyList<(string status, DateTime from, DateTime to)>> 
        GetStatusTimelineAsync(
            string machineId,
            DateTime fromUtc,
            DateTime toUtc);
}