using BaruchsTreks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection.Metadata;

public class AppDbContext : DbContext
{
    public DbSet<Trip> Trips { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define any additional configurations or constraints for your entities here
        // For example, you can specify indexes, relationships, etc.
        
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is DbEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((DbEntity)entityEntry.Entity).Modified = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((DbEntity)entityEntry.Entity).Created = DateTime.Now;
            }

            if (entityEntry.State == EntityState.Added)
            {
                ((DbEntity)entityEntry.Entity).Id = Guid.NewGuid();
            }
        }

        return base.SaveChanges();
    }
}

