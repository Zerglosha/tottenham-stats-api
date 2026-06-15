using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Clubs;

public class CreateClubRequest
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Range(0, 50)]
    public int LeagueStanding { get; set; }
    
    [StringLength(50)]
    public required string Season { get; set; }
}