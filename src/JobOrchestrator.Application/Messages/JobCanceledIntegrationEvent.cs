namespace JobOrchestrator.Application.Messages;

public record JobCanceledIntegrationEvent(Guid JobId);