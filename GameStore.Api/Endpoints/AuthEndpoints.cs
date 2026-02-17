using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using GameStore.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");

        // POST /auth/register
        group.MapPost("/register", async (RegisterDto registerDto, GameStoreContext dbContext, JwtService jwtService) =>
        {
            // Check if username already exists
            if (await dbContext.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return Results.BadRequest(new { message = "Username already exists" });
            }

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = JwtService.HashPassword(registerDto.Password)
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Generate token
            var token = jwtService.GenerateToken(user.Id, user.Username);

            return Results.Ok(new AuthResponseDto(token, user.Username));
        });

        // POST /auth/login
        group.MapPost("/login", async (LoginDto loginDto, GameStoreContext dbContext, JwtService jwtService) =>
        {
            // Find user by username
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            
            // Always verify password even if user is null to prevent timing attacks
            bool isValidPassword = false;
            if (user is not null)
            {
                isValidPassword = JwtService.VerifyPassword(loginDto.Password, user.PasswordHash);
            }
            else
            {
                // Perform a dummy password verification to maintain consistent timing
                JwtService.VerifyPassword(loginDto.Password, "$2a$11$DummyHashToPreventTimingAttack1234567890");
            }

            if (user is null || !isValidPassword)
            {
                return Results.Unauthorized();
            }

            // Generate token
            var token = jwtService.GenerateToken(user.Id, user.Username);

            return Results.Ok(new AuthResponseDto(token, user.Username));
        });
    }
}
