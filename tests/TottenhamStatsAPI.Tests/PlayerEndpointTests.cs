using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TottenhamStatsAPI.DTOs.Common;
using TottenhamStatsAPI.DTOs.Players;

namespace TottenhamStatsAPI.Tests;

public class PlayerEndpointTests : IClassFixture<TottenhamStatsApiFactory>
{
    private readonly HttpClient _client;

    public PlayerEndpointTests(TottenhamStatsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPlayers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/players");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPlayers_WithInvalidQuery_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/players?clubId=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPlayers_WithPagination_ReturnsPagedResponse()
    {
        var club = await TestApi.CreateClubAsync(_client);
        await TestApi.CreatePlayerAsync(_client, club.ClubId);
        await TestApi.CreatePlayerAsync(_client, club.ClubId);

        var response = await _client.GetAsync($"/api/players?clubId={club.ClubId}&page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PagedResponse<PlayerResponse>>(response);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetPlayerById_WhenPlayerExists_ReturnsPlayer()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var player = await TestApi.CreatePlayerAsync(_client, club.ClubId);

        var response = await _client.GetAsync($"/api/players/{player.PlayerId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PlayerResponse>(response);
        Assert.Equal(player.PlayerId, result.PlayerId);
        Assert.Equal(player.Name, result.Name);
    }

    [Fact]
    public async Task GetPlayerById_WhenPlayerDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/players/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatePlayer_WithValidRequest_ReturnsCreated()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var request = TestApi.NewPlayerRequest(club.ClubId);

        var response = await _client.PostAsJsonAsync("/api/players", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PlayerResponse>(response);
        Assert.True(result.PlayerId > 0);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Position, result.Position);
        Assert.Equal(request.SquadNumber, result.SquadNumber);
        Assert.Equal(request.Appearances, result.Appearances);
        Assert.Equal(request.Goals, result.Goals);
        Assert.Equal(request.Assists, result.Assists);
        Assert.Equal(request.IsInjured, result.IsInjured);
    }

    [Fact]
    public async Task CreatePlayer_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = TestApi.NewPlayerRequest(clubId: 0);

        var response = await _client.PostAsJsonAsync("/api/players", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePlayer_WhenPlayerExists_ReturnsUpdatedPlayer()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var player = await TestApi.CreatePlayerAsync(_client, club.ClubId);
        var request = TestApi.NewUpdatePlayerRequest(club.ClubId);

        var response = await _client.PutAsJsonAsync($"/api/players/{player.PlayerId}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PlayerResponse>(response);
        Assert.Equal(player.PlayerId, result.PlayerId);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Position, result.Position);
        Assert.Equal(request.SquadNumber, result.SquadNumber);
        Assert.Equal(request.Appearances, result.Appearances);
        Assert.Equal(request.Goals, result.Goals);
        Assert.Equal(request.Assists, result.Assists);
        Assert.Equal(request.IsInjured, result.IsInjured);
    }

    [Fact]
    public async Task UpdatePlayer_WhenPlayerDoesNotExist_ReturnsNotFound()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var request = TestApi.NewUpdatePlayerRequest(club.ClubId);

        var response = await _client.PutAsJsonAsync("/api/players/2147483000", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePlayer_WhenPlayerExists_ReturnsNoContent()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var player = await TestApi.CreatePlayerAsync(_client, club.ClubId);

        var response = await _client.DeleteAsync($"/api/players/{player.PlayerId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePlayer_WhenPlayerDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/players/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
