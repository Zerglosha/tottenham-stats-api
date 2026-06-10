using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Models;

namespace TottenhamStatsAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options) :  base (options) { }
    public DbSet<Club>  Clubs { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Match> Matches { get; set; }
}