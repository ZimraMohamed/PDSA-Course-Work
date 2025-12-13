using Microsoft.EntityFrameworkCore;
using PDSA.API.Data.Models;
using PDSA.API.Data.Models.TSP;
using PDSA.API.Data.Models.EightQueens;
using PDSA.API.Data.Models.TrafficSimulation;
using PDSA.API.Data.Models.TowerOfHanoi;
using PDSA.API.Data.Models.SnakeAndLadder;

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
        public DbSet<EQPSolution> EQPSolutions { get; set; }
        public DbSet<EQPAlgoTime> EQPAlgoTimes { get; set; }
        public DbSet<TrafficRound> TrafficRounds { get; set; }
        public DbSet<TrafficCapacity> TrafficCapacities { get; set; }
        public DbSet<TrafficAlgoTime> TrafficAlgoTimes { get; set; }
        public DbSet<HanoiRound> HanoiRounds { get; set; }
        public DbSet<HanoiAlgoTime> HanoiAlgoTimes { get; set; }
        public DbSet<SnakeLadderRound> SnakeLadderRounds { get; set; }
        public DbSet<SnakeLadderBoardConfig> SnakeLadderBoardConfigs { get; set; }
        public DbSet<SnakeLadderAlgoTime> SnakeLadderAlgoTimes { get; set; }

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

            // Configure EQPSolution entity
            modelBuilder.Entity<EQPSolution>(entity =>
            {
                entity.ToTable("EightQueens_Solutions");
                entity.HasKey(e => e.SolutionID);
                entity.Property(e => e.SolutionID).ValueGeneratedOnAdd();
                entity.Property(e => e.PlayerID).IsRequired(false);
                entity.Property(e => e.DateFound).IsRequired(false);
                entity.Property(e => e.Solution_Text).IsRequired();
                entity.Property(e => e.IsFound).IsRequired().HasDefaultValue(false);
                entity.HasIndex(e => e.Solution_Text).IsUnique();

                // Foreign key relationship
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.EQPSolutions)
                    .HasForeignKey(e => e.PlayerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure EQPAlgoTime entity
            modelBuilder.Entity<EQPAlgoTime>(entity =>
            {
                entity.ToTable("EightQueens_AlgoTimes");
                entity.HasKey(e => e.TimeID);
                entity.Property(e => e.TimeID).ValueGeneratedOnAdd();
                entity.Property(e => e.DateExecuted).IsRequired();
                entity.Property(e => e.AlgorithmType).IsRequired();
                entity.Property(e => e.TimeTaken_ms).IsRequired();
                entity.Property(e => e.RoundNumber).IsRequired();
            });

            // Configure TrafficRound entity
            modelBuilder.Entity<TrafficRound>(entity =>
            {
                entity.ToTable("TrafficSim_Rounds");
                entity.HasKey(e => e.RoundID);
                entity.Property(e => e.RoundID).ValueGeneratedOnAdd();
                entity.Property(e => e.PlayerID).IsRequired();
                entity.Property(e => e.CorrectMaxFlow).IsRequired();
                entity.Property(e => e.DatePlayed).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.TrafficRounds)
                    .HasForeignKey(e => e.PlayerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TrafficCapacity entity
            modelBuilder.Entity<TrafficCapacity>(entity =>
            {
                entity.ToTable("TrafficSim_Capacities");
                entity.HasKey(e => e.CapacityID);
                entity.Property(e => e.CapacityID).ValueGeneratedOnAdd();
                entity.Property(e => e.RoundID).IsRequired();
                entity.Property(e => e.RoadSegment).IsRequired();
                entity.Property(e => e.Capacity_VehPerMin).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Round)
                    .WithMany(r => r.Capacities)
                    .HasForeignKey(e => e.RoundID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TrafficAlgoTime entity
            modelBuilder.Entity<TrafficAlgoTime>(entity =>
            {
                entity.ToTable("TrafficSim_AlgoTimes");
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

            // Configure HanoiRound entity
            modelBuilder.Entity<HanoiRound>(entity =>
            {
                entity.ToTable("Hanoi_Rounds");
                entity.HasKey(e => e.RoundID);
                entity.Property(e => e.RoundID).ValueGeneratedOnAdd();
                entity.Property(e => e.PlayerID).IsRequired();
                entity.Property(e => e.NumDisks_N).IsRequired();
                entity.Property(e => e.NumPegs).IsRequired();
                entity.Property(e => e.CorrectMoves_Count).IsRequired();
                entity.Property(e => e.CorrectMoves_Sequence).IsRequired();
                entity.Property(e => e.DatePlayed).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.HanoiRounds)
                    .HasForeignKey(e => e.PlayerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure HanoiAlgoTime entity
            modelBuilder.Entity<HanoiAlgoTime>(entity =>
            {
                entity.ToTable("Hanoi_AlgoTimes");
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

            // Configure SnakeLadderRound entity
            modelBuilder.Entity<SnakeLadderRound>(entity =>
            {
                entity.ToTable("SnakeLadder_Rounds");
                entity.HasKey(e => e.RoundID);
                entity.Property(e => e.RoundID).ValueGeneratedOnAdd();
                entity.Property(e => e.PlayerID).IsRequired();
                entity.Property(e => e.BoardSize_N).IsRequired();
                entity.Property(e => e.NumLadders).IsRequired();
                entity.Property(e => e.NumSnakes).IsRequired();
                entity.Property(e => e.CorrectMinThrows).IsRequired();
                entity.Property(e => e.DatePlayed).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.SnakeLadderRounds)
                    .HasForeignKey(e => e.PlayerID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SnakeLadderBoardConfig entity
            modelBuilder.Entity<SnakeLadderBoardConfig>(entity =>
            {
                entity.ToTable("SnakeLadder_BoardConfig");
                entity.HasKey(e => e.ConfigID);
                entity.Property(e => e.ConfigID).ValueGeneratedOnAdd();
                entity.Property(e => e.RoundID).IsRequired();
                entity.Property(e => e.FeatureType).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Start_Cell).IsRequired();
                entity.Property(e => e.End_Cell).IsRequired();

                // Foreign key relationship
                entity.HasOne(e => e.Round)
                    .WithMany(r => r.BoardConfigs)
                    .HasForeignKey(e => e.RoundID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SnakeLadderAlgoTime entity
            modelBuilder.Entity<SnakeLadderAlgoTime>(entity =>
            {
                entity.ToTable("SnakeLadder_AlgoTimes");
                entity.HasKey(e => e.TimeID);
                entity.Property(e => e.TimeID).ValueGeneratedOnAdd();
                entity.Property(e => e.RoundID).IsRequired();
                entity.Property(e => e.AlgorithmName).IsRequired().HasMaxLength(100);
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