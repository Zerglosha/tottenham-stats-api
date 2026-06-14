namespace TottenhamStatsAPI.DTOs.Clubs;

public class CreateClubRequest
{
    public required string Name { get; set; }
    public int LeagueStanding { get; set; }
    public required string Season { get; set; }
}