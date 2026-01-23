namespace Manufacturing.Monitoring.Infrastructure.Simulation;

/// <summary>
/// Central registry of simulated machines.
/// </summary>
public static class MachineRegistry
{
    public static IReadOnlyList<string> All { get; } = new List<string>
    {
        "MACHINE-001",
        "MACHINE-002",
        "MACHINE-003",
        "MACHINE-004"
    };
}