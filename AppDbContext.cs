using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<Match> Matches { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}