namespace JobOrchestrator.Worker.Registry;

public interface IActiveJobRegistry
{
    bool TryRegistry(Guid jobId, CancellationTokenSource cancellationToken);
    void Deregister(Guid jobId);
    bool TryCancel(Guid jobId);
}
