using JobOrchestrator.Domain.Interfaces;
using JobOrchestrator.Infrastructure.Persistence;
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
            return new MongoClient(connectionString);
        });

        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = configuration["DatabaseSettings:DatabaseName"] ?? "OrchestratorDb";
            return client.GetDatabase(databaseName);
        });

        services.AddScoped<IJobRepository, JobRepository>();

        return services;
    }
}