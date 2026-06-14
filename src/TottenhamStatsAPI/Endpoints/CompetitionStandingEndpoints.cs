using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.CompetitionStandings;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.Endpoints;

public static class CompetitionStandingEndpoints
{
    public static void MapCompetitionStandingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/competition-standings");

        group.MapPost("/", CreateCompetitionStanding);
        group.MapGet("/", GetCompetitionStandings);
        group.MapGet("/{compId:int}", GetCompetitionStandingById);
        group.MapPut("/{compId:int}", UpdateCompetitionStanding);
        group.MapDelete("/{compId:int}", DeleteCompetitionStanding);
    }

    private static async Task<IResult> CreateCompetitionStanding(
        CreateCompetitionStandingRequest request, 
        AppDbContext dbContext)
    {
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
            Points = request.Points,
        };

        dbContext.CompetitionStandings.Add(comp);
        await dbContext.SaveChangesAsync();

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
            Points = comp.Points,
        };

        return Results.Created($"/api/comps/{comp.CompetitionStandingId}", response);
    }

    private static async Task<IResult> GetCompetitionStandings(AppDbContext dbContext)
    {
        var result = await dbContext.CompetitionStandings
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
                Points = comp.Points,
            })
            .ToListAsync();

        return Results.Ok(result);
    }
    
    private static async Task<IResult> GetCompetitionStandingById(int compId, AppDbContext dbContext)
    {
        var result = await dbContext.CompetitionStandings
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
                Points = comp.Points,
            })
            .SingleOrDefaultAsync();

        return result == null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> UpdateCompetitionStanding(
        int compId, 
        UpdateCompetitionStandingRequest request, 
        AppDbContext dbContext)
    {
        var comp = await dbContext.CompetitionStandings.FindAsync(compId);
        
        if (comp == null) return Results.NotFound();

        ChangeCompetitionStandingData(comp, request);
        await dbContext.SaveChangesAsync();

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
            Points = comp.Points,
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

    private static async Task<IResult> DeleteCompetitionStanding(int compId, AppDbContext dbContext)
    {
        var comp = await dbContext.CompetitionStandings.FindAsync(compId);
        if (comp ==  null) return Results.NotFound();
        
        dbContext.CompetitionStandings.Remove(comp);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
} 