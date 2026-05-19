using JobOrchestrator.Domain.Entities;

namespace JobOrchestrator.Domain.Interfaces;

public interface IWorkerJobRepository
{
    Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task UpdateAsync(Job job, CancellationToken cancellationToken);
}