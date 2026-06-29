using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Clubs;

public class ClubQueryParameters
{
    [StringLength(50)] public string? Season { get; set; }

    [StringLength(100)] public string? Search { get; set; }
}