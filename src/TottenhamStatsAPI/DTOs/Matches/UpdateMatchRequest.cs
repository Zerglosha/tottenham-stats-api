using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Matches;

public class UpdateMatchRequest
{
    [Required] [Range(1, 1000)] public int ClubId { get; set; }

    [Required] [StringLength(100)] public string Opponent { get; set; } = string.Empty;

    [DataType(DataType.Date)] public DateTime KickOffTime { get; set; }

    public bool IsHome { get; set; }

    [Required] [StringLength(100)] public string Competition { get; set; } = string.Empty;

    [Required] [StringLength(100)] public string Status { get; set; } = string.Empty;

    [Range(0, 100)] public int? TottenhamScore { get; set; }

    [Range(0, 100)] public int? OpponentScore { get; set; }
}