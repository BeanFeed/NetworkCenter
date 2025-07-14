using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public class NetworkContext : DbContext
{
    public NetworkContext() { }
    
    public NetworkContext(DbContextOptions<NetworkContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "data", "networkdata.db")}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<SocketKey>()
            .Property(p => p.Scopes)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
    }
}