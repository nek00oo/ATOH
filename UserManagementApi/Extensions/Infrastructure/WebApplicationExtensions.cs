using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace UserManagementServer.Extensions.Infrastructure;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        db.Database.Migrate();
        return app;
    }
}