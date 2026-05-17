using FluentValidation;

namespace JobOrchestrator.Application.Features.Jobs.CreateJob;

public class CreateJobValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobValidator()
    {
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("IdempotencyKey is required.")
            .MaximumLength(100).WithMessage("IdempotencyKey must not exceed 100 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid job priority.");

        RuleFor(x => x.Payload)
            .NotEmpty().WithMessage("Payload cannot be empty.");

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow).When(x => x.ScheduledAt.HasValue)
            .WithMessage("Scheduled time must be in the future.");
    }
}