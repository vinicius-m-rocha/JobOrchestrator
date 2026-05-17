using JobOrchestrator.Api.Enpoints;
using JobOrchestrator.Api.Middlewares;
using JobOrchestrator.Application;
using JobOrchestrator.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddApplicationService();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var mongoConn = builder.Configuration.GetConnectionString("MongoDb") ?? builder.Configuration["ConnectionStrings:MongoDb"];
logger.LogInformation("Effective MongoDb connection string: {MongoConn}", mongoConn);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.MapJobEndpoints();

app.Run();