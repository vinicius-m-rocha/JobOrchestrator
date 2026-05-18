using JobOrchestrator.Application.Extensions.Logs;
using JobOrchestrator.Application.Messages;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobOrchestrator.Application.Features.Jobs.Cancel;

public class CancelJobHandler(
    ILogger<CancelJobHandler> logger,
    IJobRepository jobRepository,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CancelJobCommand, bool>
{
    public async Task<bool> Handle(CancelJobCommand request, CancellationToken cancellationToken)
    {
        Job? job = await jobRepository.GetByIdAsync(request.JobId.ToString(), cancellationToken);
        if (job == null)
        {
            logger.LogJobNotFound(request.JobId);
            return false;
        }

        job.Cancel();
        await jobRepository.UpdateAsync(job, cancellationToken);

        var integrationEvent = new JobCanceledIntegrationEvent(request.JobId);
        await publishEndpoint.Publish(integrationEvent, cancellationToken);

        return true;
    }
}