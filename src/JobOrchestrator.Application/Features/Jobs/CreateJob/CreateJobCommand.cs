using JobOrchestrator.Domain.Enums;
using MediatR;

namespace JobOrchestrator.Application.Features.Jobs.CreateJob;

public record CreateJobCommand(
    string IdempotencyKey,
    JobPriority Priority,
    string Payload,
    DateTime? ScheduledAt) : IRequest<string>;