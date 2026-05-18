using JobOrchestrator.Domain.Interfaces;
using JobOrchestrator.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace JobOrchestrator.Infrastructure;

public static class InfrastrutureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MongoDbMapping.Configure();

        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            return new MongoClient(connectionString!);
        });

        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = configuration["DatabaseSettings:DatabaseName"]
                ?? throw new InvalidOperationException("Database name is not configured.");
            return client.GetDatabase(databaseName);
        });

        services
            .AddScoped<IJobRepository, JobRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddInfrastructureMessagingService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.AddMongoDbOutbox(outboxConfig =>
            {
                outboxConfig.ClientFactory(sp => sp.GetRequiredService<IMongoClient>());
                outboxConfig.DatabaseFactory(sp =>
                {
                    var client = sp.GetRequiredService<IMongoClient>();
                    var databaseName = configuration["DatabaseSettings:DatabaseName"]
                        ?? throw new InvalidOperationException("Database name is not configured.");

                    return client.GetDatabase(databaseName);
                });
                outboxConfig.UseBusOutbox();
            });

            busConfig.UsingRabbitMq((context, rabbitConfig) =>
            {
                var rabbitConnString = configuration.GetConnectionString("RabbitMq")
                    ?? throw new InvalidOperationException("RabbitMQ connection string is not configured.");

                rabbitConfig.Host(new Uri(rabbitConnString));
                rabbitConfig.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}