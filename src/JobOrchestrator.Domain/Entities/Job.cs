using JobOrchestrator.Domain.Enums;
using JobOrchestrator.Domain.ValuesObjects;

namespace JobOrchestrator.Domain.Entities;

public class Job(
    string idempotencyKey,
    JobPriority priority,
    string payload,
    string webhookUrl,
    DateTime? scheduledAt,
    JobStatus status = JobStatus.Pending)
{
    public string Id { get; private set; } = Guid.CreateVersion7().ToString();
    public string IdempotencyKey { get; private set; } = idempotencyKey;
    public JobPriority Priority { get; private set; } = priority;
    public string Payload { get; private set; } = payload;
    public string WebhookUrl { get; private set; } = webhookUrl;
    public DateTime? ScheduledAt { get; private set; } = scheduledAt;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public JobStatus Status { get; private set; } = status;

    private readonly List<ExecutionAttempt> _executionAttempts = [];
    public IReadOnlyCollection<ExecutionAttempt> ExecutionAttempts => _executionAttempts.AsReadOnly();

    public void MarkAsProcessing()
    {
        if (Status != JobStatus.Pending)
        {
            return;
        }

        Status = JobStatus.InProgress;
    }

    public void MarkAsCompleted()
    {
        if (Status != JobStatus.InProgress)
        {
            return;
        }

        Status = JobStatus.Completed;
    }

    public void MarkAsFailed()
    {
        Status = JobStatus.Failed;
    }

    public void Cancel()
    {
        if (Status == JobStatus.Completed || Status == JobStatus.Failed || Status == JobStatus.Cancelled)
        {
            return;
        }

        Status = JobStatus.Cancelled;
    }
}