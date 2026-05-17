using JobOrchestrator.Api.Endpoints.Requests;
using JobOrchestrator.Application.Features.Jobs.CreateJob;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobOrchestrator.Api.Enpoints;

public static class JobEndpoints
{
    public static void MapJobEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api;v1/jobs")
            .WithTags("Jobs");

        group.MapPost("/", CreateJob)
            .Produces(StatusCodes.Status202Accepted)
            .ProducesValidationProblem()
            .WithName("CreateJob")
            .WithSummary("Enqueues a new job for asynchronous processing");
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
}