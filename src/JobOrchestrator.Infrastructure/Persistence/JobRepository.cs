using JobOrchestrator.Application.Messages;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JobOrchestrator.Infrastructure.Persistence;

public class JobRepository(
    IMongoClient mongoClient,
    IMongoDatabase database,
    IOptions<DatabaseSettings> databaseSettings,
    IPublishEndpoint publishEndpoint) : IJobRepository
{
    private readonly IMongoCollection<Job> _collection = database.GetCollection<Job>(databaseSettings.Value.JobCollectionName);

    public async Task CreateAsync(Job job, CancellationToken cancellationToken = default)
    {
        using var session = await mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, job, cancellationToken: cancellationToken);
            var message = new JobEnqueuedEvent(job.Id, job.Priority, job.Payload, job.ScheduledAt);

            await publishEndpoint.Publish(message, cancellationToken);
            await session.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Job?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(j => j.IdempotencyKey == idempotencyKey)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}
