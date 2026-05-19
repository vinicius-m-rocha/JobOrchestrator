using JobOrchestrator.Domain.Enums;

namespace JobOrchestrator.Application.Messages;

public record JobEnqueuedEvent(
    string JobId,
    JobPriority Priority,
    string Payload,
    string WebhookUrl,
    DateTime? ScheduledAt);