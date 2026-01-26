using Manufacturing.Monitoring.Application.Services;
using Manufacturing.Monitoring.Domain.Entities;
using Manufacturing.Monitoring.Domain.Enums;
using Manufacturing.Monitoring.Tests.Fakes;
using Xunit;

namespace Manufacturing.Monitoring.Tests.Services;

public sealed class MachineMetricsServiceTests
{
        [Fact]
public async Task CalculateUptimeAsync_ReturnsUptime_WhenNeverStopped()
{
    // Arrange
    var repo = new FakeTelemetryRepository();
    var service = new MachineMetricsService(repo);

    var machineId = "MACHINE-1";
    var now = DateTime.UtcNow;

    await repo.AddAsync(new TelemetryEvent(
        machineId,
        MachineStatus.Running,
        now.AddMinutes(-30)));

    // Act
    var (uptime, downtime, currentDowntime) =
        await service.CalculateUptimeAsync(
            machineId,
            TimeSpan.FromMinutes(60));

    // Assert
    Assert.True(uptime > TimeSpan.Zero);
    Assert.Equal(TimeSpan.Zero, downtime);
    Assert.Equal(TimeSpan.Zero, currentDowntime);
}
  [Fact]
public async Task CalculateUptimeAsync_ComputesDowntime_WhenStopped()
{
    // Arrange
    var repo = new FakeTelemetryRepository();
    var service = new MachineMetricsService(repo);

    var machineId = "MACHINE-2";
    var now = DateTime.UtcNow;

    await repo.AddAsync(new TelemetryEvent(
        machineId,
        MachineStatus.Stopped,
        now.AddMinutes(-30)));

    // Act
    var (uptime, downtime, currentDowntime) =
        await service.CalculateUptimeAsync(
            machineId,
            TimeSpan.FromMinutes(31)); // ✅ avoid boundary issue

    // Assert
    Assert.Equal(TimeSpan.Zero, uptime);
    Assert.True(downtime > TimeSpan.Zero);
    Assert.True(currentDowntime > TimeSpan.Zero);
    Assert.True(currentDowntime <= downtime);
}

    [Fact]
    public async Task GetCurrentStatusAsync_ReturnsLastKnownStatus()
    {
        // Arrange
        var repo = new FakeTelemetryRepository();
        var service = new MachineMetricsService(repo);

        var machineId = "MACHINE-3";

        await repo.AddAsync(new TelemetryEvent(
            machineId,
            MachineStatus.Running,
            DateTime.UtcNow.AddMinutes(-10)));

        await repo.AddAsync(new TelemetryEvent(
            machineId,
            MachineStatus.Stopped,
            DateTime.UtcNow.AddMinutes(-1)));

        // Act
        var status = await service.GetCurrentStatusAsync(machineId);

        // Assert
        Assert.Equal(MachineStatus.Stopped, status);
    }
}