using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.DTOs.Dashboard;
using TottenhamStatsAPI.Filters;
using TottenhamStatsAPI.Helpers;

namespace TottenhamStatsAPI.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard")
            .WithDescription("Allow users to get key information about clubs");

        group.MapGet("/", GetSummary)
            .WithSummary("Get short summary")
            .AddEndpointFilter<ValidationFilter<DashboardQueryParameters>>()
            .Produces<DashboardResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> GetSummary(
        [AsParameters] DashboardQueryParameters queryParameters,
        AppDbContext dbContext,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("DashboardEndpoints");
        var clubId = queryParameters.ClubId ?? 1;
        var now = DateTime.UtcNow;

        logger.LogInformation("Dashboard summary requested for club {ClubId}", clubId);

        var club = await dbContext.Clubs
            .AsNoTracking()
            .Where(club => club.ClubId == clubId)
            .Select(club => new
            {
                club.ClubId,
                club.Name
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (club == null)
        {
            logger.LogWarning("Dashboard summary requested for missing club {ClubId}", clubId);
            return ApiErrors.NotFound("Club", clubId);
        }

        var playersCount = await dbContext.Players
            .AsNoTracking()
            .CountAsync(p => p.ClubId == clubId, cancellationToken);

        var injuredPlayersCount = await dbContext.Players
            .AsNoTracking()
            .CountAsync(p => p.ClubId == clubId && p.IsInjured, cancellationToken);

        var upcomingMatches = await dbContext.Matches
            .AsNoTracking()
            .Where(match => match.ClubId == clubId && match.KickOffTime > now)
            .OrderBy(match => match.KickOffTime)
            .Take(5)
            .Select(match =>
                new DashboardMatchResponse
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

        var lastMatches = await dbContext.Matches
            .AsNoTracking()
            .Where(match => match.ClubId == clubId && match.KickOffTime < now)
            .OrderByDescending(match => match.KickOffTime)
            .Take(5)
            .Select(match =>
                new DashboardMatchResponse
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

        var topScorers = await dbContext.Players
            .AsNoTracking()
            .Where(player => player.ClubId == clubId)
            .OrderByDescending(player => player.Goals)
            .Take(5)
            .Select(player => new DashboardPlayerStatsResponse
            {
                PlayerId = player.PlayerId,
                ClubId = player.ClubId,
                Name = player.Name,
                Position = player.Position,
                SquadNumber = player.SquadNumber,
                Appearances = player.Appearances,
                Goals = player.Goals,
                Assists = player.Assists,
                IsInjured = player.IsInjured
            })
            .ToListAsync(cancellationToken);

        var topAssists = await dbContext.Players
            .AsNoTracking()
            .Where(player => player.ClubId == clubId)
            .OrderByDescending(player => player.Assists)
            .Take(5)
            .Select(player => new DashboardPlayerStatsResponse
            {
                PlayerId = player.PlayerId,
                ClubId = player.ClubId,
                Name = player.Name,
                Position = player.Position,
                SquadNumber = player.SquadNumber,
                Appearances = player.Appearances,
                Goals = player.Goals,
                Assists = player.Assists,
                IsInjured = player.IsInjured
            })
            .ToListAsync(cancellationToken);

        var mostAppearances = await dbContext.Players
            .AsNoTracking()
            .Where(player => player.ClubId == clubId)
            .OrderByDescending(player => player.Appearances)
            .Take(5)
            .Select(player => new DashboardPlayerStatsResponse
            {
                PlayerId = player.PlayerId,
                ClubId = player.ClubId,
                Name = player.Name,
                Position = player.Position,
                SquadNumber = player.SquadNumber,
                Appearances = player.Appearances,
                Goals = player.Goals,
                Assists = player.Assists,
                IsInjured = player.IsInjured
            })
            .ToListAsync(cancellationToken);

        var response = new DashboardResponse
        {
            ClubId = clubId,
            ClubName = club.Name,
            PlayersCount = playersCount,
            InjuredPlayersCount = injuredPlayersCount,
            UpcomingMatches = upcomingMatches,
            LastMatches = lastMatches,
            TopScorers = topScorers,
            TopAssists = topAssists,
            MostAppearances = mostAppearances
        };

        logger.LogInformation(
            "Dashboard summary returned for club {ClubId}: {PlayersCount} players, {UpcomingMatchesCount} upcoming matches, {LastMatchesCount} last matches",
            clubId,
            playersCount,
            upcomingMatches.Count,
            lastMatches.Count);

        return Results.Ok(response);
    }
}