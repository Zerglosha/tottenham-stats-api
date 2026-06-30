using System.ComponentModel.DataAnnotations;
using TottenhamStatsAPI.DTOs.Common;

namespace TottenhamStatsAPI.DTOs.Players;

public class PlayerQueryParameters : PaginationParameters
{
    [Range(1, 1000)] public int? ClubId { get; set; }

    [StringLength(100)] public string? Position { get; set; }

    public bool? IsInjured { get; set; }

    [StringLength(100)] public string? Search { get; set; }
}