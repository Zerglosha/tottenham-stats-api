using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.CompetitionStandings;

public class CreateCompetitionStandingRequest
{
    [Required]
    [Range(0, 1000)]
    public int ClubId { get; set; }

    [StringLength(100)]
    public string Competition { get; set; } = string.Empty;

    [Range(0, 50)]
    public int Position { get; set; }
    
    [Range(0, 100)]
    public int Played { get; set; }
    
    [Range(0, 100)]
    public int Wins { get; set; }
    
    [Range(0, 100)]
    public int Draws { get; set; }
    
    [Range(0, 100)] 
    public int Losses { get; set; }
    
    [Range(0, 1000)]
    public int GoalsFor { get; set; }
    
    [Range(0, 1000)]
    public int GoalsAgainst { get; set; }
    
    [Range(0, 1000)]
    public int GoalDifference { get; set; }
    
    [Range(0, 300)]
    public int Points { get; set; }
}