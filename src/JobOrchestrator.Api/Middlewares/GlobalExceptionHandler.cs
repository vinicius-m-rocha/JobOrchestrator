using FluentValidation;
using JobOrchestrator.Api.Extensions.Logs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JobOrchestrator.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogUnhandledException(exception);

        if (exception is ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors ocurred.",
                Extensions =
                {
                    ["errors"] = validationException.Errors
                        .Select(e => new {
                            e.PropertyName,
                            e.ErrorMessage
                        })
                }
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails);
            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = exception.Message
            // Detail = "An unexpected error occurred. Please check the logs."
        }, cancellationToken);

        return true;
    }
}