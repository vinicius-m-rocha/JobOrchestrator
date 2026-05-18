
using JobOrchestrator.Application.Features.Jobs.CreateJob;
using JobOrchestrator.Domain.Enums;

namespace JobOrchestrator.Api.Endpoints.Requests;

public record CreateJobRequest(
    JobPriority Priority,
    string WebhookUrl,
    string Payload,
    DateTime? ScheduledAt)
{
    public CreateJobCommand ToCreateJobCommand(string idempotencyKey)
    {
        return new CreateJobCommand(
            Priority,
            Payload,
            WebhookUrl,
            ScheduledAt,
            idempotencyKey
        );
    }
}