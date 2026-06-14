using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.DTOs.Matches;

public class CreateMatchRequest
{
    public int ClubId { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public DateTime KickOffTime { get; set; }
    public bool IsHome { get; set; }
    public string Competition { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? TottenhamScore { get; set; }
    public int? OpponentScore { get; set; } 
}