namespace Manufacturing.Monitoring.Api.Dtos;

public sealed record MachineMetricsResponseDto(
    string MachineId,
    double UptimeMinutes,
    double DowntimeMinutes,
    double UptimePercentage,
    double CurrentDowntimeMinutes);