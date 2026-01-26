using Manufacturing.Monitoring.Application.Interfaces;
using Manufacturing.Monitoring.Domain.Entities;
using Manufacturing.Monitoring.Domain.Enums;

namespace Manufacturing.Monitoring.Application.Services;

public sealed class MachineHistoryService : IMachineHistoryService
{
    private readonly ITelemetryRepository _repository;

    public MachineHistoryService(ITelemetryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TelemetryEvent>> GetTelemetryAsync(
        string machineId,
        DateTime fromUtc,
        DateTime toUtc)
    {
        return await _repository.GetByMachineAsync(machineId, fromUtc, toUtc);
    }

    public async Task<IReadOnlyList<(string status, DateTime from, DateTime to)>> 
        GetStatusTimelineAsync(
            string machineId,
            DateTime fromUtc,
            DateTime toUtc)
    {
        var events = await _repository.GetByMachineAsync(machineId, fromUtc, toUtc);

        if (!events.Any())
            return Array.Empty<(string, DateTime, DateTime)>();

        var timeline = new List<(string, DateTime, DateTime)>();

        var currentStatus = events.First().Status;
        var segmentStart = fromUtc;

        foreach (var evt in events)
        {
            if (evt.Status != currentStatus)
            {
                timeline.Add((
                    currentStatus.ToString(),
                    segmentStart,
                    evt.TimestampUtc));

                currentStatus = evt.Status;
                segmentStart = evt.TimestampUtc;
            }
        }

        timeline.Add((
            currentStatus.ToString(),
            segmentStart,
            toUtc));

        return timeline;
    }
}