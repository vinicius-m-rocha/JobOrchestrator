namespace JobOrchestrator.Worker.Registry;

public interface IActiveJobRegistry
{
    bool TryRegistry(string jobId, CancellationTokenSource cancellationToken);
    void Deregister(string jobId);
    bool TryCancel(string jobId);
}
