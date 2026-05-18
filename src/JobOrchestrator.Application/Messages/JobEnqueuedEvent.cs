using JobOrchestrator.Domain.Enums;

namespace JobOrchestrator.Application.Messages;

public record JobEnqueuedEvent(
    string JobId,
    JobPriority Priority,
    string Payload,
    DateTime? ScheduledAt);