using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Players;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this WebApplication app)
    {
        app.MapPost("/api/players", async (CreatePlayerRequest request, AppDbContext dbContext) =>
        {
            var player = new Player
            {
                Name = request.Name,
                ClubId = request.ClubId,
                Position = request.Position,
                SquadNumber = request.SquadNumber,
                Appearances = request.Appearances,
                Goals = request.Goals,
                Assists = request.Assists,
                IsInjured = request.IsInjured
            };

            dbContext.Players.Add(player);
            await dbContext.SaveChangesAsync();

            var response = new PlayerResponse
            {
                PlayerId = player.PlayerId,
                Name = player.Name,
                Position = player.Position,
                SquadNumber = player.SquadNumber,
                Appearances = player.Appearances,
                Goals = player.Goals,
                Assists = player.Assists,
                IsInjured = player.IsInjured
            };

            return Results.Created($"/api/players/{player.PlayerId}", response);
        });
        
        app.MapGet("/api/players/{playerId}", async (int playerId, AppDbContext context) =>
        {
            return await context.Players
                .Where(p => p.PlayerId == playerId)
                .Select(player => new PlayerResponse
                {
                    PlayerId = player.PlayerId,
                    Name = player.Name,
                    Position = player.Position,
                    SquadNumber = player.SquadNumber,
                    Appearances = player.Appearances,
                    Goals = player.Goals,
                    Assists = player.Assists,
                    IsInjured = player.IsInjured
                })
                .SingleOrDefaultAsync();
        });

        app.MapGet("/api/players", async (AppDbContext context) =>
        {
            return await context.Players
                .Select(player => new PlayerResponse
                {
                    PlayerId = player.PlayerId,
                    Name = player.Name,
                    Position = player.Position,
                    SquadNumber = player.SquadNumber,
                    Appearances = player.Appearances,
                    Goals = player.Goals,
                    Assists = player.Assists,
                    IsInjured = player.IsInjured
                })
                .ToListAsync();
        });
    }
}