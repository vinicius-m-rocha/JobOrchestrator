using JobOrchestrator.Domain.Interfaces;
using MassTransit.MongoDbIntegration;

namespace JobOrchestrator.Infrastructure.Persistence;

public class UnitOfWork(MongoDbContext mongoDbContext) : IUnitOfWork
{
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await mongoDbContext.BeginTransaction(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await mongoDbContext.CommitTransaction(cancellationToken);
    }

    public async Task AbortTransactionAsync(CancellationToken cancellationToken = default)
    {
        await mongoDbContext.AbortTransaction(cancellationToken);
    }
}