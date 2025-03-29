using Azure.Data.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CosmosToTableMigration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting migration from Cosmos DB to Azure Table Storage...");

            // Load configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Set up Cosmos DB context
            var cosmosOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseCosmos(
                    configuration["CosmosDb:ConnectionString"],
                    configuration["CosmosDb:DatabaseName"]
                )
                .Options;

            // Set up Azure Table Storage
            var tableConnectionString = configuration["AzureStorage:ConnectionString"];
            var tableServiceClient = new TableServiceClient(tableConnectionString);
            var tableClient = tableServiceClient.GetTableClient("Trips");
            await tableClient.CreateIfNotExistsAsync();

            Console.WriteLine("Connected to Azure Table Storage. Table 'Trips' created if it didn't exist.");

            // Read all trips from Cosmos DB
            using (var cosmosContext = new AppDbContext(cosmosOptions))
            {
                Console.WriteLine("Connected to Cosmos DB. Fetching trips...");
                var trips = await cosmosContext.Trips.ToListAsync();
                Console.WriteLine($"Found {trips.Count} trips to migrate");

                int migratedCount = 0;
                int failedCount = 0;

                // Migrate each trip to Table Storage
                foreach (var trip in trips)
                {
                    try
                    {
                        var tripEntity = new TripEntity
                        {
                            PartitionKey = "Trips", // You might want a better partitioning strategy
                            RowKey = trip.id.ToString(),
                            Title = trip.Title,
                            Description = trip.Description,
                            LengthHours = trip.LengthHours,
                            ParkingJson = JsonSerializer.Serialize(trip.Parking),
                            HighPointJson = JsonSerializer.Serialize(trip.HighPoint),
                            MetersAscend = trip.MetersAscend,
                            MetersDescend = trip.MetersDescend,
                            UiaaGrade = trip.UiaaGrade?.ToString() ?? "none",
                            AlpineGrade = trip.AlpineGrade?.ToString() ?? "none",
                            TripClass = trip.TripClass?.ToString() ?? "none",
                            FerataGrade = trip.FerataGrade?.ToString() ?? "none",
                            Participants = trip.Participants ?? string.Empty,
                            TripCompletedOn = trip.TripCompletedOn?.ToString("yyyy-MM-dd") ?? string.Empty,
                            // Convert DateTime values to UTC
                            Created = DateTime.SpecifyKind(trip.Created, DateTimeKind.Utc),
                            Modified = DateTime.SpecifyKind(trip.Modified, DateTimeKind.Utc)
                        };

                        await tableClient.AddEntityAsync(tripEntity);
                        Console.WriteLine($"Migrated trip: {trip.Title} ({trip.id})");
                        migratedCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to migrate trip {trip.id}: {ex.Message}");
                        failedCount++;
                    }
                }

                Console.WriteLine($"Migration completed: {migratedCount} trips migrated successfully, {failedCount} failed.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    // Define the data models needed for migration
    public class DbEntity
    {
        public Guid id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; } = DateTime.Now;
    }

    public class Trip : DbEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal LengthHours { get; set; }
        public LocalGeometry Parking { get; set; } = new LocalGeometry();
        public LocalGeometry HighPoint { get; set; } = new LocalGeometry();
        public int MetersAscend { get; set; } = 0;
        public int MetersDescend { get; set; } = 0;
        public UiaaGradeEnum? UiaaGrade { get; set; } = UiaaGradeEnum.none;
        public AlpineGradeEnum? AlpineGrade { get; set; } = AlpineGradeEnum.none;
        public TripClassEnum? TripClass { get; set; } = TripClassEnum.none;
        public FerrataGradeEnum? FerataGrade { get; set; } = FerrataGradeEnum.none;
        public string? Participants { get; set; } = string.Empty;
        public DateOnly? TripCompletedOn { get; set; } = null;
    }

    public class LocalGeometry
    {
        public double Longtitude { get; set; }
        public double Latitude { get; set; }
    }

    // Enums needed for the migration
    public enum UiaaGradeEnum
    {
        none,
        I,
        II,
        III,
        IV,
        V,
        VI,
        VII,
        VIII,
        IX,
        X,
        XI,
        XII
    }

    public enum AlpineGradeEnum
    {
        none,
        F,
        PD,
        AD,
        D,
        TD,
        ED,
        ABO
    }

    public enum TripClassEnum
    {
        none,
        T1,
        T2,
        T3,
        T4,
        T5,
        T6
    }

    public enum FerrataGradeEnum
    {
        none,
        A,
        B,
        C,
        D,
        E
    }

    // Define the AppDbContext for Cosmos DB
    public class AppDbContext : DbContext
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

            base.OnModelCreating(modelBuilder);
        }
    }

    // Define the Table Entity class for Azure Storage Tables
    public class TripEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public Azure.ETag ETag { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal LengthHours { get; set; }
        public string ParkingJson { get; set; }
        public string HighPointJson { get; set; }
        public int MetersAscend { get; set; }
        public int MetersDescend { get; set; }
        public string UiaaGrade { get; set; }
        public string AlpineGrade { get; set; }
        public string TripClass { get; set; }
        public string FerataGrade { get; set; }
        public string Participants { get; set; }
        public string TripCompletedOn { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
