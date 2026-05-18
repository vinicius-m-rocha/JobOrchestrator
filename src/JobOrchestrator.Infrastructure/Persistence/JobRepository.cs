using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Interfaces;
using MassTransit.MongoDbIntegration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JobOrchestrator.Infrastructure.Persistence;

public class JobRepository(
    MongoDbContext mongoDbContext,
    IMongoDatabase database,
    IOptions<DatabaseSettings> databaseSettings) : IJobRepository
{
    private readonly IMongoCollection<Job> _collection = database.GetCollection<Job>(databaseSettings.Value.JobCollectionName);

    public async Task CreateAsync(Job job, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(mongoDbContext.Session, job, cancellationToken: cancellationToken);
    }

    public async Task<Job?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(mongoDbContext.Session, j => j.IdempotencyKey == idempotencyKey)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(mongoDbContext.Session, j => j.Id == id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Job job, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            mongoDbContext.Session,
            filter: j => j.Id == job.Id,
            replacement: job,
            options: new ReplaceOptions { IsUpsert = false },
            cancellationToken: cancellationToken);
    }
}