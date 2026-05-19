using JobOrchestrator.Application.Extensions.Logs;
using JobOrchestrator.Application.Messages;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobOrchestrator.Application.Features.Jobs.CreateJob;

public class CreateJobHandler(
    IJobRepository jobRepository,
    IPublishEndpoint publishEndpoint,
    IMessageScheduler messageScheduler,
    ILogger<CreateJobHandler> logger) : IRequestHandler<CreateJobCommand, string>
{
    public async Task<string> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Job? existingJob = await jobRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
            if (existingJob != null)
            {
                return existingJob.Id;
            }

            Job job = new(
                idempotencyKey: request.IdempotencyKey,
                priority: request.Priority,
                payload: request.Payload,
                webhookUrl: request.WebhookUrl,
                scheduledAt: request.ScheduledAt.GetValueOrDefault());

            await jobRepository
                .CreateAsync(job, cancellationToken);

            var jobCreatedEvent = new JobEnqueuedEvent(job.Id, job.Priority, job.Payload, job.WebhookUrl, job.ScheduledAt);

            await PublishAsync(jobCreatedEvent, cancellationToken);

            return job.Id;
        }
        catch (Exception ex)
        {
            logger.LogJobCreationError(request.IdempotencyKey, ex);
            throw;
        }
    }

    public async Task PublishAsync(JobEnqueuedEvent jobCreatedEvent, CancellationToken cancellationToken)
    {
        if (jobCreatedEvent.ScheduledAt.HasValue && jobCreatedEvent.ScheduledAt.Value > DateTime.UtcNow)
        {
            await messageScheduler.SchedulePublish(jobCreatedEvent.ScheduledAt.Value, jobCreatedEvent, cancellationToken);
        }
        else
        {
            await publishEndpoint.Publish(jobCreatedEvent, cancellationToken);
        }
    }
}