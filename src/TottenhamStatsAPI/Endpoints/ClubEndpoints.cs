using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Clubs;
using TottenhamStatsAPI.DTOs.Common;
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
            .AddEndpointFilter<ValidationFilter<ClubQueryParameters>>()
            .Produces<PagedResponse<ClubResponse>>()
            .ProducesValidationProblem();
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

    private static async Task<IResult> CreateClub(
        CreateClubRequest request,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ClubEndpoints");
        logger.LogInformation(
            "Creating club {ClubName} for season {Season}",
            request.Name,
            request.Season);

        var club = new Club
        {
            Name = request.Name,
            LeagueStanding = request.LeagueStanding,
            Season = request.Season
        };

        dbContext.Clubs.Add(club);
        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Club {ClubId} created for season {Season}",
            club.ClubId,
            club.Season);

        var response = new ClubResponse
        {
            ClubId = club.ClubId,
            Name = club.Name,
            LeagueStanding = club.LeagueStanding,
            Season = club.Season
        };

        return Results.Created($"/api/clubs/{club.ClubId}", response);
    }

    private static async Task<IResult> GetClubs(
        [AsParameters] ClubQueryParameters query,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var clubs = dbContext.Clubs
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Season)) clubs = clubs.Where(club => club.Season == query.Season);

        if (!string.IsNullOrWhiteSpace(query.Search))
            clubs = clubs.Where(club =>
                EF.Functions.ILike(club.Name, $"%{query.Search}%"));

        var page = query.CurrentPage;
        var pageSize = query.CurrentPageSize;

        var totalCount = await clubs.CountAsync(cancellationToken);

        var result = await clubs
            .OrderBy(club => club.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(club => new ClubResponse
            {
                ClubId = club.ClubId,
                Name = club.Name,
                LeagueStanding = club.LeagueStanding,
                Season = club.Season
            })
            .ToListAsync(cancellationToken);

        return Results.Ok(PagedResponse<ClubResponse>.Create(result, page, pageSize, totalCount));
    }

    private static async Task<IResult> GetClubById(
        int clubId,
        AppDbContext dbContext,
        CancellationToken cancellationToken,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ClubEndpoints");

        var result = await dbContext.Clubs
            .AsNoTracking()
            .Where(c => c.ClubId == clubId)
            .Select(club => new ClubResponse
            {
                ClubId = club.ClubId,
                Name = club.Name,
                LeagueStanding = club.LeagueStanding,
                Season = club.Season
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (result is not null) return Results.Ok(result);

        logger.LogWarning("Club {ClubId} not found", clubId);
        return ApiErrors.NotFound("Club", clubId);
    }

    private static async Task<IResult> UpdateClub(
        int clubId,
        UpdateClubRequest request,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ClubEndpoints");
        logger.LogInformation("Updating club {ClubId}", clubId);

        var club = await dbContext.Clubs.FindAsync(clubId);

        if (club == null)
        {
            logger.LogWarning("Club {ClubId} not found for update", clubId);
            return ApiErrors.NotFound("Club", clubId);
        }

        ChangeClubData(club, request);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Club {ClubId} updated", clubId);

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

    private static async Task<IResult> DeleteClub(
        int clubId,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ClubEndpoints");

        var club = await dbContext.Clubs.FindAsync(clubId);
        if (club == null)
        {
            logger.LogWarning("Club {ClubId} not found for deletion", clubId);
            return ApiErrors.NotFound("Club", clubId);
        }

        dbContext.Clubs.Remove(club);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Club {ClubId} deleted", clubId);

        return Results.NoContent();
    }
}
