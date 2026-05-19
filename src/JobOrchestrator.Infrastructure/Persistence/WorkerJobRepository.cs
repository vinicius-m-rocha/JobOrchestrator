using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JobOrchestrator.Infrastructure.Persistence;

public class WorkerJobRepository(
    IMongoDatabase database,
    IOptions<DatabaseSettings> databaseSettings) : IWorkerJobRepository
{
    private readonly IMongoCollection<Job> _collection = database.GetCollection<Job>(databaseSettings.Value.JobCollectionName);

    public async Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _collection.Find(j => j.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken)
    {
        var filter = Builders<Job>.Filter.Eq(j => j.Id, job.Id);
        await _collection.ReplaceOneAsync(filter, job, cancellationToken: cancellationToken);
    }
}