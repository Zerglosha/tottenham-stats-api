using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TottenhamStatsAPI.DTOs.Common;
using TottenhamStatsAPI.DTOs.CompetitionStandings;

namespace TottenhamStatsAPI.Tests;

public class CompetitionStandingEndpointTests : IClassFixture<TottenhamStatsApiFactory>
{
    private readonly HttpClient _client;

    public CompetitionStandingEndpointTests(TottenhamStatsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCompetitionStandings_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/competition-standings");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCompetitionStandings_WithInvalidQuery_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/competition-standings?clubId=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCompetitionStandings_WithPagination_ReturnsPagedResponse()
    {
        var club = await TestApi.CreateClubAsync(_client);
        await TestApi.CreateCompetitionStandingAsync(_client, club.ClubId);
        await TestApi.CreateCompetitionStandingAsync(_client, club.ClubId);

        var response = await _client.GetAsync(
            $"/api/competition-standings?clubId={club.ClubId}&page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PagedResponse<CompetitionStandingResponse>>(response);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetCompetitionStandings_WithFilters_ReturnsMatchingCompetitionStandings()
    {
        var token = Guid.NewGuid().ToString("N");
        var club = await TestApi.CreateClubAsync(_client);
        var otherClub = await TestApi.CreateClubAsync(_client);
        var competition = $"Filter League {token}";
        var matchingRequest = TestApi.NewCompetitionStandingRequest(club.ClubId);
        matchingRequest.Competition = competition;

        var matchingCompetitionStanding = await TestApi.CreateCompetitionStandingAsync(
            _client,
            club.ClubId,
            matchingRequest);

        var sameClubWrongCompetition = TestApi.NewCompetitionStandingRequest(club.ClubId);
        sameClubWrongCompetition.Competition = $"Other League {token}";
        await TestApi.CreateCompetitionStandingAsync(_client, club.ClubId, sameClubWrongCompetition);

        var otherClubCompetitionStanding = TestApi.NewCompetitionStandingRequest(otherClub.ClubId);
        otherClubCompetitionStanding.Competition = competition;
        await TestApi.CreateCompetitionStandingAsync(_client, otherClub.ClubId, otherClubCompetitionStanding);

        var response = await _client.GetAsync(
            $"/api/competition-standings?clubId={club.ClubId}&competition={Uri.EscapeDataString(competition)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PagedResponse<CompetitionStandingResponse>>(response);
        var competitionStanding = Assert.Single(result.Items);
        Assert.Equal(matchingCompetitionStanding.CompetitionStandingId, competitionStanding.CompetitionStandingId);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetCompetitionStandingById_WhenCompetitionStandingExists_ReturnsCompetitionStanding()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var competitionStanding = await TestApi.CreateCompetitionStandingAsync(_client, club.ClubId);

        var response = await _client.GetAsync(
            $"/api/competition-standings/{competitionStanding.CompetitionStandingId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<CompetitionStandingResponse>(response);
        Assert.Equal(competitionStanding.CompetitionStandingId, result.CompetitionStandingId);
        Assert.Equal(competitionStanding.Competition, result.Competition);
    }

    [Fact]
    public async Task GetCompetitionStandingById_WhenCompetitionStandingDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/competition-standings/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCompetitionStanding_WithValidRequest_ReturnsCreated()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var request = TestApi.NewCompetitionStandingRequest(club.ClubId);

        var response = await _client.PostAsJsonAsync("/api/competition-standings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<CompetitionStandingResponse>(response);
        Assert.True(result.CompetitionStandingId > 0);
        Assert.Equal(request.ClubId, result.ClubId);
        Assert.Equal(request.Competition, result.Competition);
        Assert.Equal(request.Position, result.Position);
        Assert.Equal(request.Played, result.Played);
        Assert.Equal(request.Wins, result.Wins);
        Assert.Equal(request.Draws, result.Draws);
        Assert.Equal(request.Losses, result.Losses);
        Assert.Equal(request.GoalsFor, result.GoalsFor);
        Assert.Equal(request.GoalsAgainst, result.GoalsAgainst);
        Assert.Equal(request.GoalDifference, result.GoalDifference);
        Assert.Equal(request.Points, result.Points);
    }

    [Fact]
    public async Task CreateCompetitionStanding_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = TestApi.NewCompetitionStandingRequest(clubId: 0);

        var response = await _client.PostAsJsonAsync("/api/competition-standings", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCompetitionStanding_WhenCompetitionStandingExists_ReturnsUpdatedCompetitionStanding()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var competitionStanding = await TestApi.CreateCompetitionStandingAsync(_client, club.ClubId);
        var request = TestApi.NewUpdateCompetitionStandingRequest(club.ClubId);

        var response = await _client.PutAsJsonAsync(
            $"/api/competition-standings/{competitionStanding.CompetitionStandingId}",
            request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<CompetitionStandingResponse>(response);
        Assert.Equal(competitionStanding.CompetitionStandingId, result.CompetitionStandingId);
        Assert.Equal(request.ClubId, result.ClubId);
        Assert.Equal(request.Competition, result.Competition);
        Assert.Equal(request.Position, result.Position);
        Assert.Equal(request.Played, result.Played);
        Assert.Equal(request.Wins, result.Wins);
        Assert.Equal(request.Draws, result.Draws);
        Assert.Equal(request.Losses, result.Losses);
        Assert.Equal(request.GoalsFor, result.GoalsFor);
        Assert.Equal(request.GoalsAgainst, result.GoalsAgainst);
        Assert.Equal(request.GoalDifference, result.GoalDifference);
        Assert.Equal(request.Points, result.Points);
    }

    [Fact]
    public async Task UpdateCompetitionStanding_WhenCompetitionStandingDoesNotExist_ReturnsNotFound()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var request = TestApi.NewUpdateCompetitionStandingRequest(club.ClubId);

        var response = await _client.PutAsJsonAsync("/api/competition-standings/2147483000", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCompetitionStanding_WhenCompetitionStandingExists_ReturnsNoContent()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var competitionStanding = await TestApi.CreateCompetitionStandingAsync(_client, club.ClubId);

        var response = await _client.DeleteAsync(
            $"/api/competition-standings/{competitionStanding.CompetitionStandingId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCompetitionStanding_WhenCompetitionStandingDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/competition-standings/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
