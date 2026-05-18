using JobOrchestrator.Worker.Consumers;
using MassTransit;

namespace JobOrchestrator.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.AddConsumer<JobEnqueuedEventConsumer>();

            busConfig.UsingRabbitMq((context, rabbitConfig) =>
            {
                var rabbitConnString = configuration.GetConnectionString("RabbitMq")
                                    ?? throw new InvalidOperationException("RabbitMQ connection string is missing.");

                rabbitConfig.Host(rabbitConnString);
                rabbitConfig.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}