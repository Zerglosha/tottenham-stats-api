namespace TottenhamStatsAPI.DTOs.Clubs;

public class UpdateClubRequest
{
    public required string Name { get; set; }
    public int LeagueStanding { get; set; }
    public required string Season { get; set; }
}