using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Common;
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
            .AddEndpointFilter<ValidationFilter<MatchQueryParameters>>()
            .Produces<PagedResponse<MatchResponse>>()
            .ProducesValidationProblem();
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

    private static async Task<IResult> CreateMatch(
        CreateMatchRequest request,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("MatchEndpoints");
        logger.LogInformation(
            "Creating match for club {ClubId} against {Opponent}",
            request.ClubId,
            request.Opponent);

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

        logger.LogInformation(
            "Match {MatchId} created for club {ClubId} against {Opponent}",
            match.MatchId,
            match.ClubId,
            match.Opponent);

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

    private static async Task<IResult> GetMatches(
        [AsParameters] MatchQueryParameters query,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var matches = dbContext.Matches
            .AsNoTracking()
            .AsQueryable();

        if (query.ClubId is not null) matches = matches.Where(match => match.ClubId == query.ClubId);

        if (!string.IsNullOrWhiteSpace(query.Competition))
            matches = matches.Where(match => match.Competition == query.Competition);

        if (!string.IsNullOrWhiteSpace(query.Status)) matches = matches.Where(match => match.Status == query.Status);

        if (query.IsHome is not null) matches = matches.Where(match => match.IsHome == query.IsHome);

        var page = query.CurrentPage;
        var pageSize = query.CurrentPageSize;

        var totalCount = await matches.CountAsync(cancellationToken);

        var result = await matches
            .OrderBy(match => match.KickOffTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<MatchResponse>
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return Results.Ok(pagedResponse);
    }

    private static async Task<IResult> GetMatchById(
        int matchId,
        AppDbContext dbContext,
        CancellationToken cancellationToken,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("MatchEndpoints");

        var result = await dbContext.Matches
            .AsNoTracking()
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
            .SingleOrDefaultAsync(cancellationToken);

        if (result is not null) return Results.Ok(result);

        logger.LogWarning("Match {MatchId} not found", matchId);
        return ApiErrors.NotFound("Match", matchId);
    }

    private static async Task<IResult> UpdateMatch(
        int matchId,
        UpdateMatchRequest request,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("MatchEndpoints");
        logger.LogInformation("Updating match {MatchId}", matchId);

        var match = await dbContext.Matches.FindAsync(matchId);

        if (match == null)
        {
            logger.LogWarning("Match {MatchId} not found for update", matchId);
            return ApiErrors.NotFound("Match", matchId);
        }

        ChangeMatchData(match, request);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Match {MatchId} updated", matchId);

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

    private static async Task<IResult> DeleteMatch(
        int matchId,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("MatchEndpoints");

        var match = await dbContext.Matches.FindAsync(matchId);
        if (match == null)
        {
            logger.LogWarning("Match {MatchId} not found for deletion", matchId);
            return ApiErrors.NotFound("Match", matchId);
        }

        dbContext.Matches.Remove(match);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Match {MatchId} deleted", matchId);

        return Results.NoContent();
    }
}
