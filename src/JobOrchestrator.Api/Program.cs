using JobOrchestrator.Api.Endpoints.Auth;
using JobOrchestrator.Api.Endpoints.Jobs;
using JobOrchestrator.Api.Extensions;
using JobOrchestrator.Api.Middlewares;
using JobOrchestrator.Application;
using JobOrchestrator.Infrastructure;
using Scalar.AspNetCore;
using JobOrchestrator.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomLogging();

builder.Services
    .AddApplicationService()
    .AddInfrastructure(builder.Configuration)
    .AddInfrastructureMessagingService(builder.Configuration);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddEndpointsApiExplorer()
    .AddOpenApiConfig()
    .AddAuthenticationService(builder.Configuration)
    .AddAuthorization();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var mongoConn = builder.Configuration.GetConnectionString("MongoDb") ?? builder.Configuration["ConnectionStrings:MongoDb"];
logger.LogInformation("Effective MongoDb connection string: {MongoConn}", mongoConn);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapJobEndpoints();
app.MapAuthEndpoints();

app.Run();