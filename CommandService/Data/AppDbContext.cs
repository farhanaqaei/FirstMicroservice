using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) {}

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Platform>()
            .HasMany(p => p.Commands)
            .WithOne(p => p.Platform!)
            .HasForeignKey(p => p.PlatformId);

        //--------------------------------------------
        // it is not necessary to define the relationship
        // between Platform and Command in both classes
        // modelBuilder
        //     .Entity<Command>()
        //     .HasOne(c => c.Platform)
        //     .WithMany(c => c.Commands)
        //     .HasForeignKey(c => c.PlatformId);
        //--------------------------------------------

        // base.OnModelCreating(modelBuilder);
    }
}
