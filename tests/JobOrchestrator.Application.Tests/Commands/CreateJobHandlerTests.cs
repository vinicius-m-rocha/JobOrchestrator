using FluentAssertions;
using JobOrchestrator.Application.Features.Jobs.CreateJob;
using JobOrchestrator.Application.Messages;
using JobOrchestrator.Application.Tests.Mocks;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobOrchestrator.Application.Tests.Commands;

public class CreateJobHandlerTests
{
    private readonly MockJobRepository _jobRepositoryMock;
    private readonly MockPublishEndpoint _publishEndpointMock;
    private readonly MockMessageScheduler _messageSchedulerMock;
    private readonly Mock<ILogger<CreateJobHandler>> _loggerMock;
    private readonly CreateJobHandler _handler;

    public CreateJobHandlerTests()
    {
        _jobRepositoryMock = new MockJobRepository();
        _publishEndpointMock = new MockPublishEndpoint();
        _messageSchedulerMock = new MockMessageScheduler();
        _loggerMock = new Mock<ILogger<CreateJobHandler>>();

        _handler = new CreateJobHandler(
            _jobRepositoryMock.Object,
            _publishEndpointMock.Object,
            _messageSchedulerMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingJob_AndDoesNotCreateOrPublish()
    {
        var command = new CreateJobCommand((JobPriority)1, "data", "http://test.com", null, Guid.NewGuid().ToString());

        var existingJob = new Job(
            idempotencyKey: "key-123",
            priority: JobPriority.Low,
            payload: "data", webhookUrl: "",
            scheduledAt: null);

        _jobRepositoryMock.MockGetByIdempotencyKeyAsync(command.IdempotencyKey, existingJob);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(existingJob.Id);

        _jobRepositoryMock
            .VerifyGetByIdempotencyKeyAsync(command.IdempotencyKey, Times.Once())
            .VerifyCreateAsync(Times.Never());

        _publishEndpointMock.VerifyPublish<JobEnqueuedEvent>(Times.Never());
        _messageSchedulerMock.VerifySchedulePublish<JobEnqueuedEvent>(Times.Never());
    }

    [Fact]
    public async Task Handle_NewJobWithoutSchedule_CreatesAndPublishesImmediately()
    {
        // Arrange
        var command = new CreateJobCommand((JobPriority)1, "test data", "http://test.com", null, Guid.NewGuid().ToString());

        _jobRepositoryMock
            .MockGetByIdempotencyKeyAsync(command.IdempotencyKey, null)
            .MockCreateAsync();

        _publishEndpointMock.MockPublish<JobEnqueuedEvent>();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();

        _jobRepositoryMock
            .VerifyGetByIdempotencyKeyAsync(command.IdempotencyKey, Times.Once())
            .VerifyCreateAsync(Times.Once());

        _publishEndpointMock.VerifyPublish<JobEnqueuedEvent>(Times.Once());
        _messageSchedulerMock.VerifySchedulePublish<JobEnqueuedEvent>(Times.Never());
    }

    [Fact]
    public async Task Handle_NewJobScheduledForFuture_CreatesAndSchedulesMessage()
    {
        // Arrange
        var scheduledTime = DateTime.UtcNow.AddHours(2);
        var command = new CreateJobCommand((JobPriority)1, "test data", "http://test.com", scheduledTime, Guid.NewGuid().ToString());

        _jobRepositoryMock
            .MockGetByIdempotencyKeyAsync(command.IdempotencyKey, null)
            .MockCreateAsync();

        _messageSchedulerMock.MockSchedulePublish<JobEnqueuedEvent>();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();

        _jobRepositoryMock
            .VerifyGetByIdempotencyKeyAsync(command.IdempotencyKey, Times.Once())
            .VerifyCreateAsync(Times.Once());

        _messageSchedulerMock.VerifySchedulePublish<JobEnqueuedEvent>(Times.Once());
        _publishEndpointMock.VerifyPublish<JobEnqueuedEvent>(Times.Never());
    }

    [Fact]
    public async Task Handle_ExceptionOccurs_ThrowsException()
    {
        // Arrange
        var command = new CreateJobCommand((JobPriority)1, "test data", "http://test.com", null, Guid.NewGuid().ToString());
        var exception = new Exception("Database failed");

        _jobRepositoryMock.MockGetByIdempotencyKeyAsyncThrows(command.IdempotencyKey, exception);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database failed");

        _jobRepositoryMock
            .VerifyGetByIdempotencyKeyAsync(command.IdempotencyKey, Times.Once())
            .VerifyCreateAsync(Times.Never());
    }
}