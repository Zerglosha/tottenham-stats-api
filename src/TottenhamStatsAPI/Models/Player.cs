namespace TottenhamStatsAPI.Models;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public int ClubId { get; set; }
    public Club Club { get; set; } = null;
    public string Position { get; set; }
    public int SquadNumber { get; set; }
    public int Appearances { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public bool IsInjured { get; set; }
}