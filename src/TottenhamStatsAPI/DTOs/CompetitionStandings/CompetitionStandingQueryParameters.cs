using System.ComponentModel.DataAnnotations;
using TottenhamStatsAPI.DTOs.Common;

namespace TottenhamStatsAPI.DTOs.CompetitionStandings;

public class CompetitionStandingQueryParameters : PaginationParameters
{
    [Range(1, 1000)] public int? ClubId { get; set; }

    [StringLength(100)] public string? Competition { get; set; }
}
