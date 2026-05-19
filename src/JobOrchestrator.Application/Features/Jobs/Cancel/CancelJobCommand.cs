using JobOrchestrator.Application.Behaviors;
using MediatR;

namespace JobOrchestrator.Application.Features.Jobs.Cancel;

public record CancelJobCommand(string JobId) : IRequest<bool>, ITransactionalCommand;