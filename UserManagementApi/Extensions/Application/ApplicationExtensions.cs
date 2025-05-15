using FluentValidation;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementApplication.Mappers;
using UserManagementApplication.Services;
using UserManagementApplication.Validators;

namespace UserManagementServer.Extensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<UserResponseMapper>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddValidators();
        
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<CreateUserDto>, CreateUserValidator>();
        services.AddSingleton<IValidator<UpdateProfileDto>, UpdateProfileUserValidator>();
        services.AddSingleton<IValidator<ChangeLoginDto>, ChangeLoginUserValidator>();
        services.AddSingleton<IValidator<ChangePasswordDto>, ChangePasswordUserValidator>();
        
        return services;
    }
}