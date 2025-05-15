using Infrastructure.Authentication;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Utils;
using UserManagementCore.Contracts;

namespace UserManagementServer.Extensions.Authentication;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordEncoder, PasswordEncoder>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        
        return services;
    }
}