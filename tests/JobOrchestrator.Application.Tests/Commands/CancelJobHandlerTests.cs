using FluentAssertions;
using JobOrchestrator.Application.Features.Jobs.Cancel;
using JobOrchestrator.Application.Messages;
using JobOrchestrator.Application.Tests.Mocks;
using JobOrchestrator.Domain.Entities;
using JobOrchestrator.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobOrchestrator.Application.Tests.Commands;

public class CancelJobHandlerTests
{
    private readonly Mock<ILogger<CancelJobHandler>> _loggerMock;
    private readonly MockJobRepository _jobRepositoryMock;
    private readonly MockPublishEndpoint _publishEndpointMock;
    private readonly CancelJobHandler _handler;

    public CancelJobHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CancelJobHandler>>();
        _jobRepositoryMock = new MockJobRepository();
        _publishEndpointMock = new MockPublishEndpoint();

        _handler = new CancelJobHandler(
            _loggerMock.Object,
            _jobRepositoryMock.Object,
            _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Handle_WhenJobDoesNotExist_ReturnFalseAndNotUpdateOrPublish()
    {
        // Arrange
        var jobId = Guid.NewGuid().ToString();
        var command = new CancelJobCommand(jobId);

        _jobRepositoryMock.MockGetByIdAsync(jobId.ToString(), null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();

        _jobRepositoryMock
            .VerifyGetByIdAsync(jobId.ToString(), Times.Once())
            .VerifyUpdateAsync(Times.Never());

        _publishEndpointMock.VerifyPublish<JobCanceledIntegrationEvent>(Times.Never());
    }

    [Fact]
    public async Task Handle_WhenJobExists_ShouldCancelAndPublishEvent()
    {
        // Arrange
        var jobId = Guid.NewGuid().ToString();
        var command = new CancelJobCommand(jobId);

        var existingJob = new Job(
            idempotencyKey: Guid.NewGuid().ToString(),
            priority: JobPriority.High,
            payload: "data",
            webhookUrl: "",
            scheduledAt: null);

        _jobRepositoryMock
            .MockGetByIdAsync(jobId.ToString(), existingJob)
            .MockUpdateAsync();

        _publishEndpointMock.MockPublish<JobCanceledIntegrationEvent>();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        existingJob.Status.Should().Be(JobStatus.Cancelled);

        _jobRepositoryMock
            .VerifyGetByIdAsync(jobId.ToString(), Times.Once())
            .VerifyUpdateAsync(Times.Once());

        _publishEndpointMock.VerifyPublish<JobCanceledIntegrationEvent>(Times.Once());
    }
}