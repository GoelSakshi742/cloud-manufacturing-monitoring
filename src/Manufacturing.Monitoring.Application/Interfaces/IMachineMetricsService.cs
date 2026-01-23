using Manufacturing.Monitoring.Domain.Enums;

namespace Manufacturing.Monitoring.Application.Interfaces;

public interface IMachineMetricsService
{
    Task<MachineStatus?> GetCurrentStatusAsync(string machineId);

    Task<(TimeSpan uptime, TimeSpan downtime, TimeSpan currentDowntime)>
        CalculateUptimeAsync(string machineId, TimeSpan window);
}