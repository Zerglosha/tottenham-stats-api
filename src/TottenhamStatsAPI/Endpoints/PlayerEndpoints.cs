using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Players;
using TottenhamStatsAPI.Filters;
using TottenhamStatsAPI.Helpers;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/players")
            .WithTags("Players")
            .WithDescription("Allow user to interact with players data in DB");

        group.MapPost("/", CreatePlayer)
            .WithSummary("Create player")
            .AddEndpointFilter<ValidationFilter<CreatePlayerRequest>>()
            .Produces<PlayerResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        group.MapGet("/", GetPlayers)
            .WithSummary("Get all players")
            .Produces<List<PlayerResponse>>();
        group.MapGet("/{playerId:int}", GetPlayerById)
            .WithSummary("Get player by ID")
            .Produces<PlayerResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{playerId:int}", UpdatePlayer)
            .WithSummary("Update player")
            .AddEndpointFilter<ValidationFilter<UpdatePlayerRequest>>()
            .Produces<PlayerResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{playerId:int}", DeletePlayer)
            .WithSummary("Delete player")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreatePlayer(CreatePlayerRequest request, AppDbContext dbContext)
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
    }

    private static async Task<IResult> GetPlayers(AppDbContext dbContext)
    {
        var result = await dbContext.Players
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

        return Results.Ok(result);
    }

    private static async Task<IResult> GetPlayerById(int playerId, AppDbContext dbContext)
    {
        var result = await dbContext.Players
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

        return result == null ? ApiErrors.NotFound("Player", playerId) : Results.Ok(result);
    }

    private static async Task<IResult> UpdatePlayer(int playerId, UpdatePlayerRequest request, AppDbContext dbContext)
    {
        var player = await dbContext.Players.FindAsync(playerId);

        if (player == null) return ApiErrors.NotFound("Player", playerId);

        ChangePlayerData(player, request);
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

        return Results.Ok(response);
    }

    private static void ChangePlayerData(Player player, UpdatePlayerRequest request)
    {
        player.Name = request.Name;
        player.ClubId = request.ClubId;
        player.Position = request.Position;
        player.SquadNumber = request.SquadNumber;
        player.Appearances = request.Appearances;
        player.Goals = request.Goals;
        player.Assists = request.Assists;
        player.IsInjured = request.IsInjured;
    }

    private static async Task<IResult> DeletePlayer(int playerId, AppDbContext dbContext)
    {
        var player = await dbContext.Players.FindAsync(playerId);
        if (player == null) return ApiErrors.NotFound("Player", playerId);

        dbContext.Players.Remove(player);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}