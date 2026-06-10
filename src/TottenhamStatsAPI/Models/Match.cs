namespace TottenhamStatsAPI.Models;

public class Match
{
    public int MatchId { get; set; }
    public int ClubId { get; set; }
    public Club Club { get; set; } = null!;
    public string Opponent { get; set; } = string.Empty;
    public DateTime KickOffTime { get; set; }
    public bool IsHome { get; set; }
    public string Competition { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? TottenhamScore { get; set; }
    public int? OpponentScore { get; set; }
}