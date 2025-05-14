using System.Reflection;
using FluentValidation;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Infrastructure.Mappers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementApplication.Extensions;
using UserManagementApplication.Mappers;
using UserManagementApplication.Services;
using UserManagementApplication.Utils;
using UserManagementApplication.Validators;
using UserManagementCore.Contracts;
using UserManagementServer.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddApplicationValidators();
services.ConfigureCustomModelValidation();
services.AddApiExtensions(configuration);
services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "API для управления пользователями. Использует JWT + Cookies аутентификацию."
    });

    c.AddSecurityDefinition("JWT_Cookie", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "token",
        Description = "JWT токен в cookie. Требуется для аутентифицированных запросов."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header (альтернативный способ аутентификации)"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT_Cookie"
                }
            },
            Array.Empty<string>()
        }
    });
});

//TODO потом попробовать сделать через OnConfiguring
// Trusted_connection=true;TrustServerCertificate=true;
services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString(nameof(UserDbContext))));

services.AddSingleton<UserPersistenceMapper>();
services.AddSingleton<UserResponseMapper>();
services.AddSingleton<IPasswordEncoder, PasswordEncoder>();
services.AddSingleton<IValidator<CreateUserDto>, CreateUserValidator>();

services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();