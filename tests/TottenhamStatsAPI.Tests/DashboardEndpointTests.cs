using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using TottenhamStatsAPI.DTOs.Dashboard;

namespace TottenhamStatsAPI.Tests;

public class DashboardEndpointTests : IClassFixture<TottenhamStatsApiFactory>
{
    private readonly HttpClient _client;

    public DashboardEndpointTests(TottenhamStatsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboard_WhenClubExists_ReturnsOk()
    {
        var club = await TestApi.CreateClubAsync(_client);

        var response = await _client.GetAsync($"/api/dashboard?clubId={club.ClubId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await TestApi.ReadRequiredJsonAsync<DashboardResponse>(response);
        Assert.Equal(club.ClubId, result.ClubId);
        Assert.Equal(club.Name, result.ClubName);
    }

    [Fact]
    public async Task GetDashboard_WhenClubDoesNotExist_ReturnsNotFound()
    {
        var missingClubId = await TestApi.GetMissingClubIdAsync(_client);

        var response = await _client.GetAsync($"/api/dashboard?clubId={missingClubId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDashboard_WithInvalidQuery_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/dashboard?clubId=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
