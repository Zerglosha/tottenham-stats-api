namespace TottenhamStatsAPI.DTOs.Dashboard;

public class DashboardResponse
{
    public int ClubId { get; set; }
    public string ClubName { get; set; } = string.Empty;

    public int PlayersCount { get; set; }
    public int InjuredPlayersCount { get; set; }

    public List<DashboardMatchResponse> UpcomingMatches { get; set; } = [];
    public List<DashboardMatchResponse> LastMatches { get; set; } = [];

    public List<DashboardPlayerStatsResponse> TopScorers { get; set; } = [];
    public List<DashboardPlayerStatsResponse> TopAssists { get; set; } = [];
    public List<DashboardPlayerStatsResponse> MostAppearances { get; set; } = [];
}