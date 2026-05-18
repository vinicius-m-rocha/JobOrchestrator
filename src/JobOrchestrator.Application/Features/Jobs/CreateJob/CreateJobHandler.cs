using JobOrchestrator.Application.Extensions.Logs;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobOrchestrator.Application.Features.Jobs.CreateJob;

public class CreateJobHandler(
    IJobRepository jobRepository,
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

            Job newJob = new(
                idempotencyKey: request.IdempotencyKey,
                priority: request.Priority,
                payload: request.Payload,
                webhookUrl: request.WebhookUrl,
                scheduledAt: request.ScheduledAt.GetValueOrDefault());

            await jobRepository
                .CreateAsync(newJob, cancellationToken);

            return newJob.Id;
        }
        catch (Exception ex)
        {
            logger.LogJobCreationError(request.IdempotencyKey, ex);
            throw;
        }
    }
}