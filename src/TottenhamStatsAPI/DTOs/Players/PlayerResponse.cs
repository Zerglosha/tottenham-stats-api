namespace TottenhamStatsAPI.DTOs.Players;

public class PlayerResponse
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int SquadNumber { get; set; }
    public int Appearances { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public bool IsInjured { get; set; }
}