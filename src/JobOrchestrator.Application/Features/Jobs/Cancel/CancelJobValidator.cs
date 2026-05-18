using FluentValidation;

namespace JobOrchestrator.Application.Features.Jobs.Cancel;

public class CancelJobValidator : AbstractValidator<CancelJobCommand>
{
    public CancelJobValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("JobId is required.");
    }
}