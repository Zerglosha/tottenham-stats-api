using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.CompetitionStandings;

public class CreateCompetitionStandingRequest
{
    [Required] [Range(1, 1000)] public int ClubId { get; set; }

    [Required] [StringLength(100)] public string Competition { get; set; } = string.Empty;

    [Required] [Range(1, 20)] public int Position { get; set; }

    [Range(0, 38)] public int Played { get; set; }

    [Range(0, 38)] public int Wins { get; set; }

    [Range(0, 38)] public int Draws { get; set; }

    [Range(0, 38)] public int Losses { get; set; }

    [Range(0, 1000)] public int GoalsFor { get; set; }

    [Range(0, 1000)] public int GoalsAgainst { get; set; }

    [Range(-1000, 1000)] public int GoalDifference { get; set; }

    [Range(0, 114)] public int Points { get; set; }
}