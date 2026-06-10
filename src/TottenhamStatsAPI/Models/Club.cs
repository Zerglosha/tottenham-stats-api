namespace TottenhamStatsAPI.Models;

public class Club
{
    public int ClubId { get; set; }
    public required string Name { get; set; }
    public int LeagueStanding { get; set; }
    public required string Season { get; set; }
    public List<Player> Players { get; set; } = [];
    public List<Match> Matches { get; set; } = [];
    public List<CompetitionStanding> CompetitionStandings { get; set; } = [];
}