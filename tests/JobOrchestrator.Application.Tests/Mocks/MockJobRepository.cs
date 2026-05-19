using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Interfaces;
using Moq;

namespace JobOrchestrator.Application.Tests.Mocks;

public class MockJobRepository : Mock<IJobRepository>
{
    #region Mock

    public MockJobRepository() : base(MockBehavior.Strict) { }

    public MockJobRepository MockCreateAsync()
    {

        Setup(x => x.CreateAsync(It.IsAny<Job>()))
            .Returns(Task.CompletedTask);

        return this;
    }
    public MockJobRepository MockGetByIdempotencyKeyAsync(string idempotencyKey, Job? result)
    {

        Setup(x => x.GetByIdempotencyKeyAsync(idempotencyKey))
            .ReturnsAsync(result);

        return this;
    }

    public MockJobRepository MockGetByIdempotencyKeyAsyncThrows(string idempotencyKey, Exception exception)
    {
        Setup(x => x.GetByIdempotencyKeyAsync(idempotencyKey, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        return this;
    }

    public MockJobRepository MockGetByIdAsync(string id, Job? result)
    {
        Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return this;
    }

    public MockJobRepository MockUpdateAsync()
    {
        Setup(x => x.UpdateAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return this;
    }

    #endregion Mock

    #region Verify

    public MockJobRepository VerifyCreateAsync(Times times)
    {

        Verify(x => x.CreateAsync(It.IsAny<Job>()), times);

        return this;
    }
    public MockJobRepository VerifyGetByIdempotencyKeyAsync(string idempotencyKey, Times times)
    {

        Verify(x => x.GetByIdempotencyKeyAsync(idempotencyKey), times);

        return this;
    }

    public MockJobRepository VerifyGetByIdAsync(string id, Times times)
    {
        Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), times);
        return this;
    }

    public MockJobRepository VerifyUpdateAsync(Times times)
    {
        Verify(x => x.UpdateAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()), times);
        return this;
    }

    #endregion Verify
}