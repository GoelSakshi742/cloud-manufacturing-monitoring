namespace Manufacturing.Monitoring.Api.Dtos;

public sealed record StatusTimelineDto(
    string Status,
    DateTime FromUtc,
    DateTime ToUtc,
    TimeSpan Duration);