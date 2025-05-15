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
        Description = "API для управления пользователями. Есть возможность использовать JWT токен через header или JWT + Cookies аутентификацию, проставляется автоматически после login."
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme"
    });
    
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        },
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
services.AddSingleton<IValidator<UpdateProfileDto>, UpdateProfileUserValidator>();
services.AddSingleton<IValidator<ChangeLoginDto>, ChangeLoginUserValidator>();
services.AddSingleton<IValidator<ChangePasswordDto>, ChangePasswordUserValidator>();

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