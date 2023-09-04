using BaruchsTreks.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Trip> Trips { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trip>()
            .HasNoDiscriminator()
            .ToContainer(nameof(Trips))
            .HasPartitionKey(da => da.id);

        modelBuilder.Entity<IdentityRole>()
            .Property(b => b.ConcurrencyStamp)
            .IsETagConcurrency();

        modelBuilder.Entity<AppUser>()
            .Property(b => b.ConcurrencyStamp)
            .IsETagConcurrency();

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        FillIdCreatedModified();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges()
    {
        FillIdCreatedModified();
        return base.SaveChanges();
    }

    private void FillIdCreatedModified()
    {
        var entries = ChangeTracker
            .Entries()
            .Where( e => e.Entity is DbEntity && (
                            e.State == EntityState.Added ||
                            e.State == EntityState.Modified
                        ));

        foreach (var entityEntry in entries)
        {
            ((DbEntity)entityEntry.Entity).Modified = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((DbEntity)entityEntry.Entity).Created = DateTime.Now;
            }
        }
    }

}

