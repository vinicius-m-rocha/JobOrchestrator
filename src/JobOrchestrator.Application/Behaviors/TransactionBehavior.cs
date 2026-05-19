using JobOrchestrator.Application.Behaviors;
using JobOrchestrator.Application.Extensions.Logs;
using JobOrchestrator.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JobOrchestrator.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(
    ILogger<TransactionBehavior<TRequest, TResponse>> logger,
    IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITransactionalCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var commandName = typeof(TRequest).Name;
        logger.LogTransactionStarted(commandName);

        try
        {
            await unitOfWork.BeginTransactionAsync();

            TResponse response = await next(cancellationToken);

            await unitOfWork.CommitTransactionAsync();

            logger.LogTransactionCommitted(commandName);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogTransactionFailed(commandName, ex);

            await unitOfWork.AbortTransactionAsync();
            throw;
        }
    }
}