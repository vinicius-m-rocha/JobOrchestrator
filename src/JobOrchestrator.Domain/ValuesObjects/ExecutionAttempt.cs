namespace JobOrchestrator.Domain.ValuesObjects;

public class ExecutionAttempt(
    int attemptNumber,
    bool isSuccess,
    string? errorMessage = null)
{
    public int Attemptnumber { get; } = attemptNumber;
    public bool IsSuccess { get; } = isSuccess;
    public string? ErrorMessage { get; } = errorMessage;
    public DateTime ExecutedAt { get; } = DateTime.UtcNow;
}