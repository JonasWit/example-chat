using Microsoft.EntityFrameworkCore;

namespace chat.service.Data;

public static class DbSetup
{
    public static void PrepareDatabase(IApplicationBuilder app)
    {
        Console.WriteLine("--> Preparing DB...");
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<ChatContext>();
        if (context is not null) ApplyMigrations(context);
    }

    private static void ApplyMigrations(ChatContext context)
    {
        Console.WriteLine("--> Attempting to apply migrations...");
        try
        {
            if (!context.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("--> No new migrations...");
                return;
            }

            Console.WriteLine("--> Applying migrations...");
            context.Database.Migrate();
            Console.WriteLine("--> Migrations applied...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not run migrations: {ex.Message}");
        }
    }
}