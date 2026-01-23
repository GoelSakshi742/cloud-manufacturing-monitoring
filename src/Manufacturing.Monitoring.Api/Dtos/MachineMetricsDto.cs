namespace Manufacturing.Monitoring.Api.Dtos;

public sealed record MachineMetricsDto(
    string MachineId,
    string CurrentStatus,
    double UptimePercentage,
    TimeSpan Uptime,
    TimeSpan Downtime,
    TimeSpan CurrentDowntimeStreak);