using Microsoft.EntityFrameworkCore;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TSP;

namespace PDSA.API.Data
{
    public class PDSADbContext : DbContext
    {
        public PDSADbContext(DbContextOptions<PDSADbContext> options) : base(options)
        {
        }

        // DbSets for the game tables
        public DbSet<Player> Players { get; set; }
        public DbSet<TSPRound> TSPRounds { get; set; }
        public DbSet<TSPDistance> TSPDistances { get; set; }
        public DbSet<TSPAlgoTime> TSPAlgoTimes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Player entity
            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(e => e.PlayerID);
                entity.Property(e => e.PlayerID).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired();
            });

            // Configure TSPRound entity
            modelBuilder.Entity<TSPRound>(entity =>
            {
                entity.ToTable("TSP_Rounds");
                entity.HasKey(e => e.RoundID);
                entity.Property(e => e.RoundID).ValueGeneratedOnAdd();
                entity.Property(e => e.PlayerID).IsRequired();
                entity.Property(e => e.HomeCity).IsRequired();
                entity.Property(e => e.SelectedCities).IsRequired();
                entity.Property(e => e.ShortestRoute_Path).IsRequired();
                entity.Property(e => e.ShortestRoute_Distance).IsRequired();
                entity.Property(e => e.DatePlayed).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.TSPRounds)
                    .HasForeignKey(e => e.PlayerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TSPDistance entity
            modelBuilder.Entity<TSPDistance>(entity =>
            {
                entity.ToTable("TSP_Distances");
                entity.HasKey(e => e.DistanceID);
                entity.Property(e => e.DistanceID).ValueGeneratedOnAdd();
                entity.Property(e => e.RoundID).IsRequired();
                entity.Property(e => e.City_A).IsRequired();
                entity.Property(e => e.City_B).IsRequired();
                entity.Property(e => e.Distance_km).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Round)
                    .WithMany(r => r.Distances)
                    .HasForeignKey(e => e.RoundID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TSPAlgoTime entity
            modelBuilder.Entity<TSPAlgoTime>(entity =>
            {
                entity.ToTable("TSP_AlgoTimes");
                entity.HasKey(e => e.TimeID);
                entity.Property(e => e.TimeID).ValueGeneratedOnAdd();
                entity.Property(e => e.RoundID).IsRequired();
                entity.Property(e => e.AlgorithmName).IsRequired();
                entity.Property(e => e.TimeTaken_ms).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Round)
                    .WithMany(r => r.AlgorithmTimes)
                    .HasForeignKey(e => e.RoundID)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}