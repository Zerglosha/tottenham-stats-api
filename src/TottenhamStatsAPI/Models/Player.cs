namespace TottenhamStatsAPI.Models;

public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public int SquadNumber { get; set; }
    public int Apperances { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public bool IsInjured { get; set; }
}