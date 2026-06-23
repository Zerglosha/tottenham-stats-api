using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Matches;

public class MatchQueryParameters
{
    [Range(1, 1000)]
    public int? ClubId { get; set; }

    [StringLength(100)]
    public string? Competition { get; set; }

    [StringLength(100)]
    public string? Status { get; set; }

    public bool? IsHome { get; set; }
}
