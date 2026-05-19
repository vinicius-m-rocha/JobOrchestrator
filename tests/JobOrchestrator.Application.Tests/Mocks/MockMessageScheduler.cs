using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobOrchestrator.Application.Messages;
using MassTransit;
using Moq;

namespace JobOrchestrator.Application.Tests.Mocks;

public class MockMessageScheduler : Mock<IMessageScheduler>
{
    public MockMessageScheduler() : base(MockBehavior.Strict) { }

    public MockMessageScheduler MockSchedulePublish<T>() where T : class
    {
        Setup(x => x.SchedulePublish(
                It.IsAny<DateTime>(),
                It.IsAny<T>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScheduledMessage<T>)null!);
        return this;
    }

    public MockMessageScheduler VerifySchedulePublish<T>(Times times) where T : class
    {
        Verify(x => x.SchedulePublish(
                It.IsAny<DateTime>(),
                It.IsAny<T>(),
                It.IsAny<CancellationToken>()),
            times);

        return this;
    }
}