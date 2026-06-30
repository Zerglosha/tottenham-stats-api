using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TottenhamStatsAPI.DTOs.Matches;

namespace TottenhamStatsAPI.Tests;

public class MatchEndpointTests : IClassFixture<TottenhamStatsApiFactory>
{
    private readonly HttpClient _client;

    public MatchEndpointTests(TottenhamStatsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMatches_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/matches");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMatches_WithInvalidQuery_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/matches?clubId=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetMatchById_WhenMatchExists_ReturnsMatch()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var match = await TestApi.CreateMatchAsync(_client, club.ClubId);

        var response = await _client.GetAsync($"/api/matches/{match.MatchId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<MatchResponse>(response);
        Assert.Equal(match.MatchId, result.MatchId);
        Assert.Equal(match.Opponent, result.Opponent);
    }

    [Fact]
    public async Task GetMatchById_WhenMatchDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/matches/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateMatch_WithValidRequest_ReturnsCreated()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var request = TestApi.NewMatchRequest(club.ClubId);

        var response = await _client.PostAsJsonAsync("/api/matches", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<MatchResponse>(response);
        Assert.True(result.MatchId > 0);
        Assert.Equal(request.ClubId, result.ClubId);
        Assert.Equal(request.Opponent, result.Opponent);
        Assert.Equal(request.IsHome, result.IsHome);
        Assert.Equal(request.Competition, result.Competition);
        Assert.Equal(request.Status, result.Status);
        Assert.Equal(request.TottenhamScore, result.TottenhamScore);
        Assert.Equal(request.OpponentScore, result.OpponentScore);
    }

    [Fact]
    public async Task CreateMatch_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = TestApi.NewMatchRequest(clubId: 0);

        var response = await _client.PostAsJsonAsync("/api/matches", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMatch_WhenMatchExists_ReturnsUpdatedMatch()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var match = await TestApi.CreateMatchAsync(_client, club.ClubId);
        var request = TestApi.NewUpdateMatchRequest(club.ClubId);

        var response = await _client.PutAsJsonAsync($"/api/matches/{match.MatchId}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<MatchResponse>(response);
        Assert.Equal(match.MatchId, result.MatchId);
        Assert.Equal(request.ClubId, result.ClubId);
        Assert.Equal(request.Opponent, result.Opponent);
        Assert.Equal(request.IsHome, result.IsHome);
        Assert.Equal(request.Competition, result.Competition);
        Assert.Equal(request.Status, result.Status);
        Assert.Equal(request.TottenhamScore, result.TottenhamScore);
        Assert.Equal(request.OpponentScore, result.OpponentScore);
    }

    [Fact]
    public async Task UpdateMatch_WhenMatchDoesNotExist_ReturnsNotFound()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var request = TestApi.NewUpdateMatchRequest(club.ClubId);

        var response = await _client.PutAsJsonAsync("/api/matches/2147483000", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMatch_WhenMatchExists_ReturnsNoContent()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var match = await TestApi.CreateMatchAsync(_client, club.ClubId);

        var response = await _client.DeleteAsync($"/api/matches/{match.MatchId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMatch_WhenMatchDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/matches/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
