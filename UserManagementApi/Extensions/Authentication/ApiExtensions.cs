using System.Text;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementServer.Extensions.Authentication;

public static class ApiExtensions
{
    public static IServiceCollection AddApiExtensions(this IServiceCollection services, IConfiguration config)
    {
        var jwtOptions = config.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var tokenFromHeader = context.Request.Headers["Authorization"]
                            .FirstOrDefault()?
                            .Split(" ")
                            .Last();
                        
                        var tokenFromCookie = context.Request.Cookies["token"];

                        context.Token = tokenFromHeader ?? tokenFromCookie;
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policyBuilder =>
            {
                policyBuilder.RequireClaim("Admin", "true");
            });
        });

        return services;
    }

}