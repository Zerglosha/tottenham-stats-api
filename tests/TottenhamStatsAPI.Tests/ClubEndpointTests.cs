using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TottenhamStatsAPI.DTOs.Clubs;
using TottenhamStatsAPI.DTOs.Common;

namespace TottenhamStatsAPI.Tests;

public class ClubEndpointTests : IClassFixture<TottenhamStatsApiFactory>
{
    private readonly HttpClient _client;

    public ClubEndpointTests(TottenhamStatsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetClubs_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/clubs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetClubs_WithInvalidQuery_ReturnsBadRequest()
    {
        var tooLongSearch = new string('a', 101);

        var response = await _client.GetAsync($"/api/clubs?search={tooLongSearch}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetClubs_WithPagination_ReturnsPagedResponse()
    {
        var season = $"test-{Guid.NewGuid():N}";
        await TestApi.CreateClubAsync(_client, TestApi.NewClubRequest(season: season));
        await TestApi.CreateClubAsync(_client, TestApi.NewClubRequest(season: season));

        var response = await _client.GetAsync($"/api/clubs?season={Uri.EscapeDataString(season)}&page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PagedResponse<ClubResponse>>(response);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetClubs_WithFilters_ReturnsMatchingClubs()
    {
        var token = Guid.NewGuid().ToString("N");
        var season = $"filter-season-{token}";
        var matchingClub = await TestApi.CreateClubAsync(
            _client,
            TestApi.NewClubRequest(name: $"Filter Club {token}", season: season));
        await TestApi.CreateClubAsync(_client, TestApi.NewClubRequest(name: "Other Club", season: season));
        await TestApi.CreateClubAsync(
            _client,
            TestApi.NewClubRequest(name: $"Filter Club {token}", season: $"other-{token}"));

        var response = await _client.GetAsync(
            $"/api/clubs?season={Uri.EscapeDataString(season)}&search={token}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<PagedResponse<ClubResponse>>(response);
        var club = Assert.Single(result.Items);
        Assert.Equal(matchingClub.ClubId, club.ClubId);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetClubById_WhenClubExists_ReturnsClub()
    {
        var club = await TestApi.CreateClubAsync(_client);

        var response = await _client.GetAsync($"/api/clubs/{club.ClubId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<ClubResponse>(response);
        Assert.Equal(club.ClubId, result.ClubId);
        Assert.Equal(club.Name, result.Name);
    }

    [Fact]
    public async Task GetClubById_WhenClubDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/clubs/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateClub_WithValidRequest_ReturnsCreated()
    {
        var request = TestApi.NewClubRequest();

        var response = await _client.PostAsJsonAsync("/api/clubs", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<ClubResponse>(response);
        Assert.True(result.ClubId > 0);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.LeagueStanding, result.LeagueStanding);
        Assert.Equal(request.Season, result.Season);
    }

    [Fact]
    public async Task CreateClub_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = TestApi.NewClubRequest(leagueStanding: 0);

        var response = await _client.PostAsJsonAsync("/api/clubs", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateClub_WhenClubExists_ReturnsUpdatedClub()
    {
        var club = await TestApi.CreateClubAsync(_client);
        var request = TestApi.NewUpdateClubRequest();

        var response = await _client.PutAsJsonAsync($"/api/clubs/{club.ClubId}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<ClubResponse>(response);
        Assert.Equal(club.ClubId, result.ClubId);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.LeagueStanding, result.LeagueStanding);
        Assert.Equal(request.Season, result.Season);
    }

    [Fact]
    public async Task UpdateClub_WhenClubDoesNotExist_ReturnsNotFound()
    {
        var request = TestApi.NewUpdateClubRequest();

        var response = await _client.PutAsJsonAsync("/api/clubs/2147483000", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClub_WhenClubExists_ReturnsNoContent()
    {
        var club = await TestApi.CreateClubAsync(_client);

        var response = await _client.DeleteAsync($"/api/clubs/{club.ClubId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClub_WhenClubDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/clubs/2147483000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
