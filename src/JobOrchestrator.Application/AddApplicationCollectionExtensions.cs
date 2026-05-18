using FluentValidation;
using JobOrchestrator.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace JobOrchestrator.Application;

public static class AddApplicationCollectionExtensions
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        var assembly = typeof(AddApplicationCollectionExtensions).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            cfg
                .AddOpenBehavior(typeof(ValidationBehavior<,>))
                .AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}