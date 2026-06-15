using System.ComponentModel.DataAnnotations;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.DTOs.Matches;

public class CreateMatchRequest
{
    [Required]
    [Range(0, 1000)]
    public int ClubId { get; set; }
    
    [StringLength(1000)]
    public string Opponent { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime KickOffTime { get; set; }
    
    public bool IsHome { get; set; }
    
    [StringLength(100)]
    public string Competition { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Status { get; set; } = string.Empty;
    
    public int? TottenhamScore { get; set; }
    public int? OpponentScore { get; set; } 
}