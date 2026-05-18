using JobOrchestrator.Application.Messages;
using MassTransit;
using JobOrchestrator.Worker.Extensions.Logs;

namespace JobOrchestrator.Worker.Consumers;

public class JobEnqueuedEventConsumer(
    ILogger<JobEnqueuedEventConsumer> logger) : IConsumer<JobEnqueuedEvent>
{
    public async Task Consume(ConsumeContext<JobEnqueuedEvent> context)
    {
        var message = context.Message;

        logger.LogProcessingStarted(message.JobId, message.Priority.ToString());

        try
        {
            // Simulating real business logic workload
            await Task.Delay(3000, context.CancellationToken);

            logger.LogProcessingSuccess(message.JobId);
        }
        catch (Exception ex)
        {
            logger.LogProcessingFailed(message.JobId, ex);
            throw;
        }
    }
}