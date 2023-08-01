using BaruchsTreks.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext
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
            .HasPartitionKey(da => da.Id);

        modelBuilder.Entity<IdentityRole>()
            .Property(b => b.ConcurrencyStamp)
            .IsETagConcurrency();

        modelBuilder.Entity<IdentityUser>()
            .Property(b => b.ConcurrencyStamp)
            .IsETagConcurrency();

        base.OnModelCreating(modelBuilder);

        var roleGuid = Guid.NewGuid();
        var userGuid = Guid.NewGuid();
        modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Id = roleGuid.ToString(), Name = "Administrator", NormalizedName = "ADMINISTRATOR".ToUpper() });
        var hasher = new PasswordHasher<IdentityUser>();
        //Seeding the User to AspNetUsers table
        modelBuilder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = userGuid.ToString(), // primary key
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PasswordHash = hasher.HashPassword(null, "pwd")
            }
        );


        //Seeding the relation between our user and role to AspNetUserRoles table
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = roleGuid.ToString(),
                UserId = userGuid.ToString()
            }
        );

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

            if (entityEntry.State == EntityState.Added)
            {
                ((DbEntity)entityEntry.Entity).Id = Guid.NewGuid();
            }
        }
    }

}

