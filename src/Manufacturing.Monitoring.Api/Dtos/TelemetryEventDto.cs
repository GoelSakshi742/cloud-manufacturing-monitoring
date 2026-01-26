namespace Manufacturing.Monitoring.Api.Dtos;

public sealed record TelemetryEventDto(
    string MachineId,
    string Status,
    DateTime TimestampUtc);