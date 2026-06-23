using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Players;

public class CreatePlayerRequest
{
    [Required] [StringLength(100)] public string Name { get; set; } = string.Empty;

    [Required] [Range(1, 1000)] public int ClubId { get; set; }

    [Required] [StringLength(100)] public string Position { get; set; } = string.Empty;

    [Range(1, 100)] public int SquadNumber { get; set; }

    [Range(0, 200)] public int Appearances { get; set; }

    [Range(0, 200)] public int Goals { get; set; }

    [Range(0, 200)] public int Assists { get; set; }

    public bool IsInjured { get; set; }
}