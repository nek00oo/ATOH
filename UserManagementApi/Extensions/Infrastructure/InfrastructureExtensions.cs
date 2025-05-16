using Infrastructure.Mappers;
using Infrastructure.Repositories;
using UserManagementCore.Contracts;

namespace UserManagementServer.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserPersistenceMapper, UserPersistenceMapper>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        
        return services;
    }
}