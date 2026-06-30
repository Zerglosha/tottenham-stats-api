using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Common;
using TottenhamStatsAPI.DTOs.CompetitionStandings;
using TottenhamStatsAPI.Filters;
using TottenhamStatsAPI.Helpers;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.Endpoints;

public static class CompetitionStandingEndpoints
{
    public static void MapCompetitionStandingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/competition-standings")
            .WithTags("Competition Standings")
            .WithDescription("Allow users to interact with competition standings data in DB");

        group.MapPost("/", CreateCompetitionStanding)
            .WithSummary("Create competition standing")
            .AddEndpointFilter<ValidationFilter<CreateCompetitionStandingRequest>>()
            .Produces<CompetitionStandingResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        group.MapGet("/", GetCompetitionStandings)
            .WithSummary("Get all competition standings")
            .AddEndpointFilter<ValidationFilter<CompetitionStandingQueryParameters>>()
            .Produces<PagedResponse<CompetitionStandingResponse>>()
            .ProducesValidationProblem();
        group.MapGet("/{compId:int}", GetCompetitionStandingById)
            .WithSummary("Get competition standing by ID")
            .Produces<CompetitionStandingResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{compId:int}", UpdateCompetitionStanding)
            .WithSummary("Update competition standing")
            .AddEndpointFilter<ValidationFilter<UpdateCompetitionStandingRequest>>()
            .Produces<CompetitionStandingResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{compId:int}", DeleteCompetitionStanding)
            .WithSummary("Delete competition standing")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateCompetitionStanding(
        CreateCompetitionStandingRequest request,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("CompetitionStandingEndpoints");
        logger.LogInformation(
            "Creating competition standing for club {ClubId} in {Competition}",
            request.ClubId,
            request.Competition);

        var comp = new CompetitionStanding
        {
            ClubId = request.ClubId,
            Competition = request.Competition,
            Position = request.Position,
            Played = request.Played,
            Wins = request.Wins,
            Draws = request.Draws,
            Losses = request.Losses,
            GoalsFor = request.GoalsFor,
            GoalsAgainst = request.GoalsAgainst,
            GoalDifference = request.GoalDifference,
            Points = request.Points
        };

        dbContext.CompetitionStandings.Add(comp);
        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Competition standing {CompetitionStandingId} created for club {ClubId} in {Competition}",
            comp.CompetitionStandingId,
            comp.ClubId,
            comp.Competition);

        var response = new CompetitionStandingResponse
        {
            CompetitionStandingId = comp.CompetitionStandingId,
            ClubId = comp.ClubId,
            Competition = comp.Competition,
            Position = comp.Position,
            Played = comp.Played,
            Wins = comp.Wins,
            Draws = comp.Draws,
            Losses = comp.Losses,
            GoalsFor = comp.GoalsFor,
            GoalsAgainst = comp.GoalsAgainst,
            GoalDifference = comp.GoalDifference,
            Points = comp.Points
        };

        return Results.Created($"/api/competition-standings/{comp.CompetitionStandingId}", response);
    }

    private static async Task<IResult> GetCompetitionStandings(
        [AsParameters] CompetitionStandingQueryParameters query,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var competitionStandings = dbContext.CompetitionStandings
            .AsNoTracking()
            .AsQueryable();

        if (query.ClubId is not null)
            competitionStandings = competitionStandings.Where(comp => comp.ClubId == query.ClubId);

        if (!string.IsNullOrWhiteSpace(query.Competition))
            competitionStandings = competitionStandings.Where(comp => comp.Competition == query.Competition);

        var page = query.CurrentPage;
        var pageSize = query.CurrentPageSize;

        var totalCount = await competitionStandings.CountAsync(cancellationToken);

        var result = await competitionStandings
            .OrderBy(comp => comp.Competition)
            .ThenBy(comp => comp.Position)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(comp => new CompetitionStandingResponse
            {
                CompetitionStandingId = comp.CompetitionStandingId,
                ClubId = comp.ClubId,
                Competition = comp.Competition,
                Position = comp.Position,
                Played = comp.Played,
                Wins = comp.Wins,
                Draws = comp.Draws,
                Losses = comp.Losses,
                GoalsFor = comp.GoalsFor,
                GoalsAgainst = comp.GoalsAgainst,
                GoalDifference = comp.GoalDifference,
                Points = comp.Points
            })
            .ToListAsync(cancellationToken);

        return Results.Ok(PagedResponse<CompetitionStandingResponse>.Create(result, page, pageSize, totalCount));
    }

    private static async Task<IResult> GetCompetitionStandingById(
        int compId,
        AppDbContext dbContext,
        CancellationToken cancellationToken,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("CompetitionStandingEndpoints");

        var result = await dbContext.CompetitionStandings
            .AsNoTracking()
            .Where(c => c.CompetitionStandingId == compId)
            .Select(comp => new CompetitionStandingResponse
            {
                CompetitionStandingId = comp.CompetitionStandingId,
                ClubId = comp.ClubId,
                Competition = comp.Competition,
                Position = comp.Position,
                Played = comp.Played,
                Wins = comp.Wins,
                Draws = comp.Draws,
                Losses = comp.Losses,
                GoalsFor = comp.GoalsFor,
                GoalsAgainst = comp.GoalsAgainst,
                GoalDifference = comp.GoalDifference,
                Points = comp.Points
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (result is not null) return Results.Ok(result);

        logger.LogWarning("Competition standing {CompetitionStandingId} not found", compId);
        return ApiErrors.NotFound("Competition Standing", compId);
    }

    private static async Task<IResult> UpdateCompetitionStanding(
        int compId,
        UpdateCompetitionStandingRequest request,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("CompetitionStandingEndpoints");
        logger.LogInformation("Updating competition standing {CompetitionStandingId}", compId);

        var comp = await dbContext.CompetitionStandings.FindAsync(compId);

        if (comp == null)
        {
            logger.LogWarning("Competition standing {CompetitionStandingId} not found for update", compId);
            return ApiErrors.NotFound("Competition Standing", compId);
        }

        ChangeCompetitionStandingData(comp, request);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Competition standing {CompetitionStandingId} updated", compId);

        var response = new CompetitionStandingResponse
        {
            CompetitionStandingId = comp.CompetitionStandingId,
            ClubId = comp.ClubId,
            Competition = comp.Competition,
            Position = comp.Position,
            Played = comp.Played,
            Wins = comp.Wins,
            Draws = comp.Draws,
            Losses = comp.Losses,
            GoalsFor = comp.GoalsFor,
            GoalsAgainst = comp.GoalsAgainst,
            GoalDifference = comp.GoalDifference,
            Points = comp.Points
        };

        return Results.Ok(response);
    }

    private static void ChangeCompetitionStandingData(
        CompetitionStanding competitionStanding,
        UpdateCompetitionStandingRequest request)
    {
        competitionStanding.ClubId = request.ClubId;
        competitionStanding.Competition = request.Competition;
        competitionStanding.Position = request.Position;
        competitionStanding.Played = request.Played;
        competitionStanding.Wins = request.Wins;
        competitionStanding.Draws = request.Draws;
        competitionStanding.Losses = request.Losses;
        competitionStanding.GoalsFor = request.GoalsFor;
        competitionStanding.GoalsAgainst = request.GoalsAgainst;
        competitionStanding.GoalDifference = request.GoalDifference;
        competitionStanding.Points = request.Points;
    }

    private static async Task<IResult> DeleteCompetitionStanding(
        int compId,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("CompetitionStandingEndpoints");

        var comp = await dbContext.CompetitionStandings.FindAsync(compId);
        if (comp == null)
        {
            logger.LogWarning("Competition standing {CompetitionStandingId} not found for deletion", compId);
            return ApiErrors.NotFound("Competition Standing", compId);
        }

        dbContext.CompetitionStandings.Remove(comp);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Competition standing {CompetitionStandingId} deleted", compId);

        return Results.NoContent();
    }
}
