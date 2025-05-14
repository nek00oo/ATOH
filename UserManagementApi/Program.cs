using FluentValidation;
using Infrastructure.Auth;
using Infrastructure.Data;
using Infrastructure.Mappers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddControllers();
builder.Services.AddApplicationValidators();
builder.Services.ConfigureCustomModelValidation();
builder.Services.AddApiExtensions(builder.Configuration);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TODO потом попробовать сделать через OnConfiguring
// Trusted_connection=true;TrustServerCertificate=true;
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(UserDbContext))));

builder.Services.AddSingleton<UserPersistenceMapper>();
builder.Services.AddSingleton<UserResponseMapper>();
builder.Services.AddSingleton<IPasswordEncoder, PasswordEncoder>();
builder.Services.AddSingleton<IValidator<CreateUserDto>, CreateUserValidator>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUserService, UserService>();

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