using Infrastructure.Authentication;
using Infrastructure.Data;
using Infrastructure.Entities;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using UserManagementApplication.Extensions;
using UserManagementCore.Contracts;
using UserManagementCore.Types;
using UserManagementServer.Extensions;
using UserManagementServer.Extensions.Application;
using UserManagementServer.Extensions.Authentication;
using UserManagementServer.Extensions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers(options => 
{
    options.OutputFormatters.Add(new StreamOutputFormatter());
});

services.AddApplicationValidators();
services.ConfigureCustomModelValidation();
services
    .AddApiExtensions(configuration)
    .AddInfrastructureServices()
    .AddApplicationServices()
    .AddAuthServices();

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString(nameof(UserDbContext))));

services.AddCustomSwagger();

var app = builder.Build();
app.MigrateDatabase();

//Добавление админа admin:admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    var passwordEncoder = scope.ServiceProvider.GetRequiredService<IPasswordEncoder>();
    
    if (!db.Users.Any(u => u.Admin))
    {
        var adminUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Login = "admin",
            Password = passwordEncoder.HashPassword("admin"),
            Name = "Administrator",
            Gender = Gender.Man,
            Admin = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "system",
            ModifiedOn = DateTime.UtcNow,
            ModifiedBy = "system"
        };

        db.Users.Add(adminUser);
        await db.SaveChangesAsync();
    }
}

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