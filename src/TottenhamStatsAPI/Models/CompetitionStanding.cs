namespace TottenhamStatsAPI.Models;

public class CompetitionStanding
{
    public int CompetitionStandingId { get; set; }

    public int ClubId { get; set; }
    public Club Club { get; set; } = null!;

    public string Competition { get; set; } = string.Empty;

    public int Position { get; set; }
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
    public int Points { get; set; }
}