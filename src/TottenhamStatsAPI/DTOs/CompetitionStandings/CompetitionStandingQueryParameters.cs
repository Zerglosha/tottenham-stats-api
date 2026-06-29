using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.CompetitionStandings;

public class CompetitionStandingQueryParameters
{
    [Range(1, 1000)] public int? ClubId { get; set; }

    [StringLength(100)] public string? Competition { get; set; }
}