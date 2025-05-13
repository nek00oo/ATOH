using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Mappers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementApplication.Abstractions;
using UserManagementApplication.Dto;
using UserManagementApplication.Extensions;
using UserManagementApplication.Services;
using UserManagementApplication.Utils;
using UserManagementApplication.Validators;
using UserManagementCore.Contracts;
using UserManagementServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationValidators();
builder.Services.ConfigureCustomModelValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//TODO потом попробовать сделать через OnConfiguring
// Trusted_connection=true;TrustServerCertificate=true;
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(UserDbContext))));

builder.Services.AddSingleton<UserMapper>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IPasswordEncoder, PasswordEncoder>();
builder.Services.AddSingleton<IValidator<CreateUserDto>, CreateUserValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();