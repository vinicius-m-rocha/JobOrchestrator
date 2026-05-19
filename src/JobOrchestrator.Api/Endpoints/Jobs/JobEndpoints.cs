using JobOrchestrator.Api.Endpoints.Jobs.Requests;
using JobOrchestrator.Application.Features.Jobs.Cancel;
using JobOrchestrator.Application.Features.Jobs.CreateJob;
using JobOrchestrator.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobOrchestrator.Api.Endpoints.Jobs;

public static class JobEndpoints
{
    public static void MapJobEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/jobs")
            .RequireAuthorization()
            .WithTags("Jobs");

        group.MapPost("/", CreateJob)
            .Produces(StatusCodes.Status202Accepted)
            .ProducesValidationProblem()
            .WithName("CreateJob")
            .WithSummary("Enqueues a new job for asynchronous processing");

        group.MapDelete("/{jobId:guid}", CancelJob)
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("CancelJob")
            .WithSummary("Requests cancellation of a job that is currently being processed or is queued for processing");
    }

    public static async Task<IResult> CreateJob(
        [FromBody] CreateJobRequest request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        CreateJobCommand command = request.ToCreateJobCommand(idempotencyKey);
        string jobId = await mediator.Send(command, cancellationToken);

        return Results.Created($"/api/v1/jobs/{jobId}", new { JobId = jobId });
    }

    public static async Task<IResult> CancelJob(
        [FromRoute] string jobId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        CancelJobCommand command = new(jobId);
        bool result = await mediator.Send(command, cancellationToken);

        if (!result)
        {
            return Results.NotFound(new { Message = "Job not found." });
        }

        return Results.Accepted(value: new { Message = "Cancellation requested successfully." });
    }
}