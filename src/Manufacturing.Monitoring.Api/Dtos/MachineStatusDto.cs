namespace Manufacturing.Monitoring.Api.Dtos;

public sealed record MachineStatusDto(
    string MachineId,
    string Status,
    DateTime TimestampUtc);