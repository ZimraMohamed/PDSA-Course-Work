using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TSP
{
    [Table("TSP_Rounds")]
    public class TSPRound
    {
        [Key]
        [Column("RoundID")]
        public int RoundID { get; set; }

        [Required]
        [Column("PlayerID")]
        public int PlayerID { get; set; }

        [Required]
        [Column("HomeCity")]
        public string HomeCity { get; set; } = string.Empty;

        [Required]
        [Column("SelectedCities")]
        public string SelectedCities { get; set; } = string.Empty;

        [Required]
        [Column("ShortestRoute_Path")]
        public string ShortestRoute_Path { get; set; } = string.Empty;

        [Required]
        [Column("ShortestRoute_Distance")]
        public double ShortestRoute_Distance { get; set; }

        [Required]
        [Column("DatePlayed")]
        public string DatePlayed { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }

        public virtual ICollection<TSPDistance> Distances { get; set; } = new List<TSPDistance>();
        public virtual ICollection<TSPAlgoTime> AlgorithmTimes { get; set; } = new List<TSPAlgoTime>();
    }
}
