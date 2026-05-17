using JobOrchestrator.Domain.Enums;
using MediatR;

namespace JobOrchestrator.Application.Features.Jobs.CreateJob;

public record CreateJobCommand(
    JobPriority Priority,
    string Payload,
    DateTime? ScheduledAt,
    string IdempotencyKey) : IRequest<string>;