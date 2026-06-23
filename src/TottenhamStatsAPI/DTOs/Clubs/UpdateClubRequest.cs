using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Clubs;

public class UpdateClubRequest
{
    [Required] [StringLength(100)] public required string Name { get; set; }

    [Required] [Range(1, 20)] public int LeagueStanding { get; set; }

    [Required] [StringLength(50)] public required string Season { get; set; }
}