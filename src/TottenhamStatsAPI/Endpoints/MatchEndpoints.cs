using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Matches;
using TottenhamStatsAPI.Filters;
using TottenhamStatsAPI.Helpers;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.Endpoints;

public static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/matches")
            .WithTags("Matches")
            .WithDescription("Allow users to interact with the matches data in DB");

        group.MapPost("/", CreateMatch)
            .WithSummary("Create match")
            .AddEndpointFilter<ValidationFilter<CreateMatchRequest>>()
            .Produces<MatchResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        group.MapGet("/", GetMatches)
            .WithSummary("Get all matches")
            .Produces<List<MatchResponse>>();
        group.MapGet("/{matchId:int}", GetMatchById)
            .WithSummary("Get match by ID")
            .Produces<MatchResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{matchId:int}", UpdateMatch)
            .WithSummary("Update match")
            .AddEndpointFilter<ValidationFilter<UpdateMatchRequest>>()
            .Produces<MatchResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{matchId:int}", DeleteMatch)
            .WithSummary("Delete match")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateMatch(CreateMatchRequest request, AppDbContext dbContext)
    {
        var match = new Match
        {
            ClubId = request.ClubId,
            Opponent = request.Opponent,
            KickOffTime = request.KickOffTime,
            IsHome = request.IsHome,
            Competition = request.Competition,
            Status = request.Status,
            TottenhamScore = request.TottenhamScore,
            OpponentScore = request.OpponentScore
        };

        dbContext.Matches.Add(match);
        await dbContext.SaveChangesAsync();

        var response = new MatchResponse
        {
            MatchId = match.MatchId,
            ClubId = match.ClubId,
            Opponent = match.Opponent,
            KickOffTime = match.KickOffTime,
            IsHome = match.IsHome,
            Competition = match.Competition,
            Status = match.Status,
            TottenhamScore = match.TottenhamScore,
            OpponentScore = match.OpponentScore
        };

        return Results.Created($"/api/matches/{match.MatchId}", response);
    }

    private static async Task<IResult> GetMatches(AppDbContext dbContext)
    {
        var result = await dbContext.Matches
            .Select(match => new MatchResponse
            {
                MatchId = match.MatchId,
                ClubId = match.ClubId,
                Opponent = match.Opponent,
                KickOffTime = match.KickOffTime,
                IsHome = match.IsHome,
                Competition = match.Competition,
                Status = match.Status,
                TottenhamScore = match.TottenhamScore,
                OpponentScore = match.OpponentScore
            })
            .ToListAsync();

        return Results.Ok(result);
    }

    private static async Task<IResult> GetMatchById(int matchId, AppDbContext dbContext)
    {
        var result = await dbContext.Matches
            .Where(m => m.MatchId == matchId)
            .Select(match => new MatchResponse
            {
                MatchId = match.MatchId,
                ClubId = match.ClubId,
                Opponent = match.Opponent,
                KickOffTime = match.KickOffTime,
                IsHome = match.IsHome,
                Competition = match.Competition,
                Status = match.Status,
                TottenhamScore = match.TottenhamScore,
                OpponentScore = match.OpponentScore
            })
            .SingleOrDefaultAsync();

        return result == null ? ApiErrors.NotFound("Match", matchId) : Results.Ok(result);
    }

    private static async Task<IResult> UpdateMatch(int matchId, UpdateMatchRequest request, AppDbContext dbContext)
    {
        var match = await dbContext.Matches.FindAsync(matchId);

        if (match == null) return ApiErrors.NotFound("Match", matchId);

        ChangeMatchData(match, request);
        await dbContext.SaveChangesAsync();

        var response = new MatchResponse
        {
            MatchId = match.MatchId,
            ClubId = match.ClubId,
            Opponent = match.Opponent,
            KickOffTime = match.KickOffTime,
            IsHome = match.IsHome,
            Competition = match.Competition,
            Status = match.Status,
            TottenhamScore = match.TottenhamScore,
            OpponentScore = match.OpponentScore
        };

        return Results.Ok(response);
    }

    private static void ChangeMatchData(Match match, UpdateMatchRequest request)
    {
        match.ClubId = request.ClubId;
        match.Opponent = request.Opponent;
        match.KickOffTime = request.KickOffTime;
        match.IsHome = request.IsHome;
        match.Competition = request.Competition;
        match.Status = request.Status;
        match.TottenhamScore = request.TottenhamScore;
        match.OpponentScore = request.OpponentScore;
    }

    private static async Task<IResult> DeleteMatch(int matchId, AppDbContext dbContext)
    {
        var match = await dbContext.Matches.FindAsync(matchId);
        if (match == null) return ApiErrors.NotFound("Match", matchId);

        dbContext.Matches.Remove(match);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}