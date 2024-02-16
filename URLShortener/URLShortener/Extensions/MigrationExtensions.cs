using Microsoft.EntityFrameworkCore;
using URLShortener.Context;

namespace URLShortener.Extensions;

public static class MigrationExtensions
{
    public static void Migrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ContextBase>();

        dbContext.Database.Migrate();
    }
}
