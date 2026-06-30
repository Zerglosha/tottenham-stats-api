using System.Net;
using System.Net.Http.Json;
using TottenhamStatsAPI.DTOs.Clubs;
using TottenhamStatsAPI.DTOs.CompetitionStandings;
using TottenhamStatsAPI.DTOs.Matches;
using TottenhamStatsAPI.DTOs.Players;

namespace TottenhamStatsAPI.Tests;

internal static class TestApi
{
    internal static CreateClubRequest NewClubRequest(
        string? name = null,
        int leagueStanding = 6,
        string season = "2025/26")
    {
        return new CreateClubRequest
        {
            Name = name ?? NewName("Test Club"),
            LeagueStanding = leagueStanding,
            Season = season
        };
    }

    internal static UpdateClubRequest NewUpdateClubRequest(
        string? name = null,
        int leagueStanding = 4,
        string season = "2026/27")
    {
        return new UpdateClubRequest
        {
            Name = name ?? NewName("Updated Club"),
            LeagueStanding = leagueStanding,
            Season = season
        };
    }

    internal static CreatePlayerRequest NewPlayerRequest(int clubId, string? name = null)
    {
        return new CreatePlayerRequest
        {
            Name = name ?? NewName("Test Player"),
            ClubId = clubId,
            Position = "Forward",
            SquadNumber = 11,
            Appearances = 12,
            Goals = 5,
            Assists = 3,
            IsInjured = false
        };
    }

    internal static UpdatePlayerRequest NewUpdatePlayerRequest(int clubId, string? name = null)
    {
        return new UpdatePlayerRequest
        {
            Name = name ?? NewName("Updated Player"),
            ClubId = clubId,
            Position = "Midfielder",
            SquadNumber = 8,
            Appearances = 18,
            Goals = 6,
            Assists = 9,
            IsInjured = true
        };
    }

    internal static CreateMatchRequest NewMatchRequest(int clubId, string? opponent = null)
    {
        return new CreateMatchRequest
        {
            ClubId = clubId,
            Opponent = opponent ?? NewName("Test Opponent"),
            KickOffTime = DateTime.UtcNow.AddDays(7),
            IsHome = true,
            Competition = "Premier League",
            Status = "Scheduled",
            TottenhamScore = null,
            OpponentScore = null
        };
    }

    internal static UpdateMatchRequest NewUpdateMatchRequest(int clubId, string? opponent = null)
    {
        return new UpdateMatchRequest
        {
            ClubId = clubId,
            Opponent = opponent ?? NewName("Updated Opponent"),
            KickOffTime = DateTime.UtcNow.AddDays(-1),
            IsHome = false,
            Competition = "FA Cup",
            Status = "Finished",
            TottenhamScore = 2,
            OpponentScore = 1
        };
    }

    internal static CreateCompetitionStandingRequest NewCompetitionStandingRequest(int clubId)
    {
        return new CreateCompetitionStandingRequest
        {
            ClubId = clubId,
            Competition = NewName("Test Competition"),
            Position = 5,
            Played = 20,
            Wins = 10,
            Draws = 5,
            Losses = 5,
            GoalsFor = 35,
            GoalsAgainst = 24,
            GoalDifference = 11,
            Points = 35
        };
    }

    internal static UpdateCompetitionStandingRequest NewUpdateCompetitionStandingRequest(int clubId)
    {
        return new UpdateCompetitionStandingRequest
        {
            ClubId = clubId,
            Competition = NewName("Updated Competition"),
            Position = 3,
            Played = 22,
            Wins = 13,
            Draws = 5,
            Losses = 4,
            GoalsFor = 42,
            GoalsAgainst = 25,
            GoalDifference = 17,
            Points = 44
        };
    }

    internal static async Task<ClubResponse> CreateClubAsync(HttpClient client, CreateClubRequest? request = null)
    {
        var response = await client.PostAsJsonAsync("/api/clubs", request ?? NewClubRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadRequiredJsonAsync<ClubResponse>(response);
    }

    internal static async Task<PlayerResponse> CreatePlayerAsync(
        HttpClient client,
        int clubId,
        CreatePlayerRequest? request = null)
    {
        var response = await client.PostAsJsonAsync("/api/players", request ?? NewPlayerRequest(clubId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadRequiredJsonAsync<PlayerResponse>(response);
    }

    internal static async Task<MatchResponse> CreateMatchAsync(
        HttpClient client,
        int clubId,
        CreateMatchRequest? request = null)
    {
        var response = await client.PostAsJsonAsync("/api/matches", request ?? NewMatchRequest(clubId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadRequiredJsonAsync<MatchResponse>(response);
    }

    internal static async Task<CompetitionStandingResponse> CreateCompetitionStandingAsync(
        HttpClient client,
        int clubId,
        CreateCompetitionStandingRequest? request = null)
    {
        var response = await client.PostAsJsonAsync(
            "/api/competition-standings",
            request ?? NewCompetitionStandingRequest(clubId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadRequiredJsonAsync<CompetitionStandingResponse>(response);
    }

    internal static async Task<int> GetMissingClubIdAsync(HttpClient client)
    {
        foreach (var clubId in Enumerable.Range(1, 1000))
        {
            var response = await client.GetAsync($"/api/clubs/{clubId}");

            if (response.StatusCode == HttpStatusCode.NotFound) return clubId;
        }

        throw new InvalidOperationException("Could not find a missing club id in the valid dashboard range.");
    }

    internal static async Task<T> ReadRequiredJsonAsync<T>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<T>();

        Assert.NotNull(result);
        return result;
    }

    private static string NewName(string prefix)
    {
        return $"{prefix} {Guid.NewGuid():N}";
    }
}
