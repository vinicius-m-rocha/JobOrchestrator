using JobOrchestrator.Infrastructure;
using JobOrchestrator.Infrastructure.Logging;
using JobOrchestrator.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.AddCustomLogging();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddWorkerServices(builder.Configuration)
    .AddRetryPolicy();

var host = builder.Build();
host.Run();