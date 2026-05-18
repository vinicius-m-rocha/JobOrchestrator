using JobOrchestrator.Infrastructure;
using JobOrchestrator.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddWorkerServices(builder.Configuration);

var host = builder.Build();
host.Run();