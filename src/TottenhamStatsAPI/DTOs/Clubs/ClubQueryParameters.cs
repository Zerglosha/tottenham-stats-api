using System.ComponentModel.DataAnnotations;
using TottenhamStatsAPI.DTOs.Common;

namespace TottenhamStatsAPI.DTOs.Clubs;

public class ClubQueryParameters : PaginationParameters
{
    [StringLength(50)] public string? Season { get; set; }

    [StringLength(100)] public string? Search { get; set; }
}
