using JobOrchestrator.Domain.Entities;

namespace JobOrchestrator.Domain.Interfaces;

public interface IJobRepository
{
    Task CreateAsync(Job job, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);
}