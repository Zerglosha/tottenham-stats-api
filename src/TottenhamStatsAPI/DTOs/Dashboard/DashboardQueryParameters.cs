using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.DTOs.Dashboard;

public class DashboardQueryParameters
{
    [Range(1, 1000)]
    public int? ClubId { get; set; }
}
