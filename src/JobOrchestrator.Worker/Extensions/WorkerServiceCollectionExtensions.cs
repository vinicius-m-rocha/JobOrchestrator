using MassTransit;
using JobOrchestrator.Worker.Consumers;
using JobOrchestrator.Worker.Registry;

namespace JobOrchestrator.Worker.Extensions;

public static class WorkerServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IActiveJobRegistry, ActiveJobRegistry>();

        services.AddHttpClient();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<JobEnqueuedEventConsumer>();
            x.AddConsumer<JobCanceledIntegrationEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitConnString = configuration.GetConnectionString("RabbitMq")
                                ?? throw new InvalidOperationException("RabbitMQ connection string is missing.");
                cfg.Host(rabbitConnString);
                cfg.UseDelayedMessageScheduler();
                cfg.ReceiveEndpoint("job-execution-queue", e =>
                {
                    e.PrefetchCount = 1;
                    e.EnablePriority(10);
                    e.ConfigureConsumer<JobEnqueuedEventConsumer>(context);
                });

                var instanceId = Guid.NewGuid().ToString("N");
                cfg.ReceiveEndpoint($"job-cancellation-queue-{instanceId}", e =>
                {
                    e.AutoDelete = true;
                    e.ConfigureConsumer<JobCanceledIntegrationEventConsumer>(context);
                });
            });
        });

        return services;
    }
}