using JobOrchestrator.Application.Messages;
using MassTransit;
using JobOrchestrator.Worker.Extensions.Logs;
using JobOrchestrator.Domain.Interfaces;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Enums;
using System.Text;
using JobOrchestrator.Worker.Registry;

namespace JobOrchestrator.Worker.Consumers;

public class JobEnqueuedEventConsumer(
    ILogger<JobEnqueuedEventConsumer> logger,
    IActiveJobRegistry activeJobRegistry,
    IHttpClientFactory httpClientFactory,
    IJobRepository jobRepository) : IConsumer<JobEnqueuedEvent>
{
    public async Task Consume(ConsumeContext<JobEnqueuedEvent> context)
    {
        JobEnqueuedEvent message = context.Message;
        string jobId = message.JobId.ToString();

        var job = await jobRepository.GetByIdAsync(jobId, context.CancellationToken);

        if (job is null || job.Status == JobStatus.Cancelled)
            return;

        logger.LogProcessingStarted(jobId, message.Priority.ToString());

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
            logger.LogJobAborted(jobId, ex);
        }
        catch (Exception ex)
        {
            logger.LogProcessingFailed(jobId, ex);
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