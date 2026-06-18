using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Clubs;
using TottenhamStatsAPI.Filters;
using TottenhamStatsAPI.Helpers;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.Endpoints;

public static class ClubEndpoints
{
    public static void MapClubEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/clubs")
            .WithTags("Clubs")
            .WithDescription("Allow users to interact with clubs data in DB");

        group.MapPost("/", CreateClub)
            .WithSummary("Create club")
            .AddEndpointFilter<ValidationFilter<CreateClubRequest>>()
            .Produces<ClubResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        group.MapGet("/", GetClubs)
            .WithSummary("Get all clubs")
            .Produces<List<ClubResponse>>();
        group.MapGet("/{clubId:int}", GetClubById)
            .WithSummary("Get club by ID")
            .Produces<ClubResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPut("/{clubId:int}", UpdateClub)
            .WithSummary("Update club")
            .AddEndpointFilter<ValidationFilter<UpdateClubRequest>>()
            .Produces<ClubResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{clubId:int}", DeleteClub)
            .WithSummary("Delete club")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateClub(CreateClubRequest request, AppDbContext dbContext)
    {
        var club = new Club
        {
            Name = request.Name,
            LeagueStanding = request.LeagueStanding,
            Season = request.Season
        };

        dbContext.Clubs.Add(club);
        await dbContext.SaveChangesAsync();

        var response = new ClubResponse
        {
            ClubId = club.ClubId,
            Name = club.Name,
            LeagueStanding = club.LeagueStanding,
            Season = club.Season
        };

        return Results.Created($"/api/clubs/{club.ClubId}", response);
    }

    private static async Task<IResult> GetClubs(AppDbContext dbContext)
    {
        var result = await dbContext.Clubs
            .Select(club => new ClubResponse
            {
                ClubId = club.ClubId,
                Name = club.Name,
                LeagueStanding = club.LeagueStanding,
                Season = club.Season
            })
            .ToListAsync();

        return Results.Ok(result);
    }

    private static async Task<IResult> GetClubById(int clubId, AppDbContext dbContext)
    {
        var result = await dbContext.Clubs
            .Where(c => c.ClubId == clubId)
            .Select(club => new ClubResponse
            {
                ClubId = club.ClubId,
                Name = club.Name,
                LeagueStanding = club.LeagueStanding,
                Season = club.Season
            })
            .SingleOrDefaultAsync();

        return result == null ? ApiErrors.NotFound("Club", clubId) : Results.Ok(result);
    }

    private static async Task<IResult> UpdateClub(int clubId, UpdateClubRequest request, AppDbContext dbContext)
    {
        var club = await dbContext.Clubs.FindAsync(clubId);

        if (club == null) return ApiErrors.NotFound("Club", clubId);

        ChangeClubData(club, request);
        await dbContext.SaveChangesAsync();

        var response = new ClubResponse
        {
            ClubId = club.ClubId,
            Name = club.Name,
            LeagueStanding = club.LeagueStanding,
            Season = club.Season
        };

        return Results.Ok(response);
    }

    private static void ChangeClubData(Club club, UpdateClubRequest request)
    {
        club.Name = request.Name;
        club.LeagueStanding = request.LeagueStanding;
        club.Season = request.Season;
    }

    private static async Task<IResult> DeleteClub(int clubId, AppDbContext dbContext)
    {
        var club = await dbContext.Clubs.FindAsync(clubId);
        if (club == null) return ApiErrors.NotFound("Club", clubId);

        dbContext.Clubs.Remove(club);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}