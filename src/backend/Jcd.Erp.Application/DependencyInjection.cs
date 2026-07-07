using FluentValidation;
using Jcd.Erp.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Jcd.Erp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(assembly);

        return services;
    }
}
