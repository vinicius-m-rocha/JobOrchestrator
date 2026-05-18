using JobOrchestrator.Application.Behaviors;
using MediatR;

namespace JobOrchestrator.Application.Features.Jobs.Cancel;

public record CancelJobCommand(Guid JobId) : IRequest<bool>, ITransactionalCommand;