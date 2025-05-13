using Microsoft.AspNetCore.Mvc;

namespace UserManagementServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCustomModelValidation(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return new BadRequestObjectResult(new { errors });
            };
        });

        return services;
    }
}