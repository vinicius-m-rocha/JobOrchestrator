using JobOrchestrator.Application.Messages;
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
        return await _collection.Find(j => j.IdempotencyKey == idempotencyKey)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}