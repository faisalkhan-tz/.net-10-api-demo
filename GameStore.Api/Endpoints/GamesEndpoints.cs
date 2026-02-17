using System;
using System.Security.Claims;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{

    const string GetGameEndPointName = "GetGame";

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
            .WithTags("Games")
            .RequireAuthorization();

        // GET /games - List games owned by authenticated user
        group.MapGet("/", async (GameStoreContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var games = await dbContext.Games
                .Include(game => game.Genre)
                .Where(game => game.OwnerId == userId)
                .Select(game => new GameSummaryDto(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.Price,
                    game.ReleaseDate
                ))
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(games);
        });

        // GET /games/{id} - Get one game by ID (only if owned by user)
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var game = await dbContext.Games.FindAsync(id);
            
            if (game is null)
            {
                return Results.NotFound();
            }
            
            if (game.OwnerId != userId)
            {
                return Results.Forbid();
            }
            
            return Results.Ok(new GameDetailsDto(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            ));
        }).WithName(GetGameEndPointName);

        // POST /games - Create a game (assigns current user as owner)
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate,
                OwnerId = userId
            };

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            GameDetailsDto gameDetails = new(game.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate);

            return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, gameDetails);
        });

        // PUT /games/{id} - Update a game (only if owned by user)
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var existingGame = await dbContext.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }
            
            if (existingGame.OwnerId != userId)
            {
                return Results.Forbid();
            }

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;

            await dbContext.SaveChangesAsync();

            return Results.Ok(existingGame);
        });

        // DELETE /games/{id} - Delete a game (only if owned by user)
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext, ClaimsPrincipal user) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var gameToDelete = await dbContext.Games.FindAsync(id);
            if (gameToDelete is null)
            {
                return Results.NotFound();
            }
            
            if (gameToDelete.OwnerId != userId)
            {
                return Results.Forbid();
            }

            dbContext.Games.Remove(gameToDelete);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }

}
