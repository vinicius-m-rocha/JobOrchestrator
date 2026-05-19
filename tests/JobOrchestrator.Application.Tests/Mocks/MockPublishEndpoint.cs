
using JobOrchestrator.Application.Messages;
using MassTransit;
using Moq;

namespace JobOrchestrator.Application.Tests.Mocks;

public class MockPublishEndpoint : Mock<IPublishEndpoint>
{
    public MockPublishEndpoint() : base(MockBehavior.Strict) { }

    public MockPublishEndpoint MockPublish<T>() where T : class
    {
        Setup(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return this;
    }

    public MockPublishEndpoint VerifyPublish<T>(Times times) where T : class
    {
        Verify(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()), times);
        return this;
    }
}