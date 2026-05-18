using JobOrchestrator.Application.Messages;
using JobOrchestrator.Worker.Extensions.Logs;
using JobOrchestrator.Worker.Registry;
using MassTransit;

namespace JobOrchestrator.Worker.Consumers;

public class JobCanceledIntegrationEventConsumer(
    IActiveJobRegistry activeJobRegistry,
    ILogger<JobCanceledIntegrationEventConsumer> logger)
    : IConsumer<JobCanceledIntegrationEvent>
{
    public Task Consume(ConsumeContext<JobCanceledIntegrationEvent> context)
    {
        var jobId = context.Message.JobId;

        if (activeJobRegistry.TryCancel(jobId))
        {
            logger.LogKillSwitchActivated(jobId);
        }

        return Task.CompletedTask;
    }
}