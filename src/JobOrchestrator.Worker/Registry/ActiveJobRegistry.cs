
using System.Collections.Concurrent;

namespace JobOrchestrator.Worker.Registry;

public class ActiveJobRegistry : IActiveJobRegistry
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeJobs = new();

    public bool TryRegistry(Guid jobId, CancellationTokenSource cancellationTokenSource)
    {
        return _activeJobs.TryAdd(jobId, cancellationTokenSource);
    }

    public void Deregister(Guid jobId)
    {
        if (_activeJobs.TryRemove(jobId, out var cts))
        {
            cts.Dispose();
        }
    }

    public bool TryCancel(Guid jobId)
    {
        if (_activeJobs.TryGetValue(jobId, out var cts))
        {
            if (!cts.IsCancellationRequested)
            {
                cts.Cancel();
            }
            return true;
        }

        return false;
    }
}