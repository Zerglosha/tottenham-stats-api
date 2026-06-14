namespace TottenhamStatsAPI.DTOs.Clubs;

public class ClubResponse
{
    public int ClubId { get; set; }
    public required string Name { get; set; }
    public int LeagueStanding { get; set; }
    public required string Season { get; set; }
}