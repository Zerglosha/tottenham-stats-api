namespace TottenhamStatsAPI.Models;

public class Club
{
    public int ClubId { get; set; }
    public required string Name { get; set; }
    public int LeagueStanding { get; set; }
    public required string Season { get; set; }
}