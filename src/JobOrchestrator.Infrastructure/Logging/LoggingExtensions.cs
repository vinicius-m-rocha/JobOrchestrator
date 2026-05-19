using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JobOrchestrator.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static void AddCustomLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
                .Enrich.WithEnvironmentName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        });
    }

    public static void AddCustomLogging(this HostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
                .Enrich.WithEnvironmentName();
        });
    }
}