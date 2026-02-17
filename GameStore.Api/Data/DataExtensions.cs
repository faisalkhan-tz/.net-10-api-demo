using System;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static void MigrateDB(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        db.Database.Migrate();
    }

    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("GameStore") ?? "Data Source=GameStore.db";
        builder.Services.AddScoped<GameStoreContext>();   
        builder.Services.AddSqlite<GameStoreContext>(connectionString, optionsAction: options => options.UseSeeding((ctx, _) =>
        {

            if (!ctx.Set<Genre>().Any())
            {
                ctx.Set<Genre>().AddRange(
                    new Genre { Name = "Action" },
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "RPG" },
                    new Genre { Name = "Strategy" },
                    new Genre { Name = "Sports" },
                    new Genre { Name = "Racing" },
                    new Genre { Name = "Fighting" },
                    new Genre { Name = "Puzzle" }
                );
                ctx.SaveChanges();
            }

        }));
    }
}
