namespace TottenhamStatsAPI.DTOs.Players;

public class CreatePlayerRequest
{
    public string Name { get; set; } = string.Empty;
    public int ClubId { get; set; }
    public string Position { get; set; } = string.Empty;
    public int SquadNumber { get; set; }
    public int Appearances { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public bool IsInjured { get; set; }
}