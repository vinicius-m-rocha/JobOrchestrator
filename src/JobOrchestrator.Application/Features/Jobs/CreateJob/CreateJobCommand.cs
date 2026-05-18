using JobOrchestrator.Application.Behaviors;
using JobOrchestrator.Domain.Enums;
using MediatR;

namespace JobOrchestrator.Application.Features.Jobs.CreateJob;

public record CreateJobCommand(
    JobPriority Priority,
    string Payload,
    string WebhookUrl,
    DateTime? ScheduledAt,
    string IdempotencyKey) : IRequest<string>, ITransactionalCommand;