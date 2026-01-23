using Manufacturing.Monitoring.Application.Interfaces;
using Manufacturing.Monitoring.Infrastructure.Repositories;
using Manufacturing.Monitoring.Infrastructure.Simulation;
using Manufacturing.Monitoring.Application.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITelemetryRepository, InMemoryTelemetryRepository>();
builder.Services.AddHostedService<TelemetrySimulationService>();
builder.Services.AddScoped<IMachineMetricsService, MachineMetricsService>();
var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();