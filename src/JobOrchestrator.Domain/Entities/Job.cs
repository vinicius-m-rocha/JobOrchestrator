using JobOrchestrator.Domain.Enums;
using JobOrchestrator.Domain.ValuesObjects;

namespace JobOrchestrator.Domain.Entities;

public class Job(
    string idempotencyKey,
    JobPriority priority,
    string payload,
    DateTime? scheduledAt,
    JobStatus status = JobStatus.Pending)
{
    public string Id { get; private set; } = Guid.CreateVersion7().ToString();
    public string IdempotencyKey { get; private set; } = idempotencyKey;
    public JobPriority Priority { get; private set; } = priority;
    public string Payload { get; private set; } = payload;
    public DateTime? ScheduledAt { get; private set; } = scheduledAt;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public JobStatus Status { get; private set; } = status;

    private readonly List<ExecutionAttempt> _executionAttempts = [];
    public IReadOnlyCollection<ExecutionAttempt> ExecutionAttempts => _executionAttempts.AsReadOnly();
}