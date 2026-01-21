using Manufacturing.Monitoring.Domain.Enums;

namespace Manufacturing.Monitoring.Domain.Entities;

/// <summary>
/// Represents a single telemetry event emitted by a machine.
/// Immutable and time‑ordered.
/// </summary>
public sealed class TelemetryEvent
{
    public string MachineId { get; }
    public MachineStatus Status { get; }
    public DateTime TimestampUtc { get; }

    public TelemetryEvent(
        string machineId,
        MachineStatus status,
        DateTime timestampUtc)
    {
        if (string.IsNullOrWhiteSpace(machineId))
            throw new ArgumentException("MachineId cannot be null or empty.", nameof(machineId));

        if (timestampUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Timestamp must be in UTC.", nameof(timestampUtc));

        MachineId = machineId;
        Status = status;
        TimestampUtc = timestampUtc;
    }
}