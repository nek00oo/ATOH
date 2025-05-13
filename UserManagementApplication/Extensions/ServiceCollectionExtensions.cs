using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace UserManagementApplication.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddFluentValidationAutoValidation();

        return services;
    }
}