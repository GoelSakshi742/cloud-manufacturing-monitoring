using Manufacturing.Monitoring.Application.Interfaces;
using Manufacturing.Monitoring.Domain.Entities;
using Manufacturing.Monitoring.Domain.Enums;
using Microsoft.Extensions.Hosting;

namespace Manufacturing.Monitoring.Infrastructure.Simulation;

/// <summary>
/// Simulates machine telemetry by emitting periodic status events.
/// </summary>
public sealed class TelemetrySimulationService : BackgroundService
{
    private readonly ITelemetryRepository _repository;
    private readonly Random _random = new();

    private readonly Dictionary<string, MachineStatus> _lastStatus = new();

    public TelemetrySimulationService(ITelemetryRepository repository)
    {
        _repository = repository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var machineId in MachineRegistry.All)
            {
                var nextStatus = GetNextStatus(machineId);

                var telemetryEvent = new TelemetryEvent(
                    machineId,
                    nextStatus,
                    DateTime.UtcNow);

                await _repository.AddAsync(telemetryEvent);
            }

            // Emit telemetry every 10 seconds
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private MachineStatus GetNextStatus(string machineId)
    {
        if (!_lastStatus.TryGetValue(machineId, out var current))
        {
            current = MachineStatus.Running;
        }

        // 80% chance to remain in same state
        var shouldFlip = _random.NextDouble() < 0.2;

        var next = shouldFlip
            ? (current == MachineStatus.Running
                ? MachineStatus.Stopped
                : MachineStatus.Running)
            : current;

        _lastStatus[machineId] = next;
        return next;
    }
}