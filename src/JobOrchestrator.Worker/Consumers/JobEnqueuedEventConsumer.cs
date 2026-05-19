using JobOrchestrator.Application.Messages;
using MassTransit;
using JobOrchestrator.Worker.Extensions.Logs;
using JobOrchestrator.Domain.Interfaces;
using JobOrchestrator.Domain.Enums;
using System.Text;
using JobOrchestrator.Worker.Registry;

namespace JobOrchestrator.Worker.Consumers;

public class JobEnqueuedEventConsumer(
    ILogger<JobEnqueuedEventConsumer> logger,
    IActiveJobRegistry activeJobRegistry,
    IHttpClientFactory httpClientFactory,
    IWorkerJobRepository jobRepository) : IConsumer<JobEnqueuedEvent>
{
    public async Task Consume(ConsumeContext<JobEnqueuedEvent> context)
    {
        JobEnqueuedEvent message = context.Message;

        var job = await jobRepository.GetByIdAsync(message.JobId, context.CancellationToken);

        if (job is null || job.Status == JobStatus.Cancelled)
            return;

        logger.LogProcessingStarted(message.JobId, message.Priority.ToString());

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);

            job.MarkAsProcessing();
            await jobRepository.UpdateAsync(job, cts.Token);

            activeJobRegistry.TryRegistry(message.JobId, cts);

            HttpClient httpClient = httpClientFactory.CreateClient("WebhookClient");
            var httpContent = new StringContent(message.Payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(message.WebhookUrl, httpContent, cts.Token);

            response.EnsureSuccessStatusCode();

            job.MarkAsCompleted();
        }
        catch (OperationCanceledException ex)
        {
            job.Cancel();
            logger.LogJobAborted(message.JobId, ex);
        }
        catch (Exception ex)
        {
            logger.LogProcessingFailed(message.JobId, ex);
            job.MarkAsFailed();
            throw;
        }
        finally
        {
            activeJobRegistry.Deregister(message.JobId);
            await jobRepository.UpdateAsync(job, CancellationToken.None);
        }
    }
}