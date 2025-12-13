using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TSP
{
    [Table("TSP_AlgoTimes")]
    public class TSPAlgoTime
    {
        [Key]
        [Column("TimeID")]
        public int TimeID { get; set; }

        [Required]
        [Column("RoundID")]
        public int RoundID { get; set; }

        [Required]
        [Column("AlgorithmName")]
        public string AlgorithmName { get; set; } = string.Empty;

        [Required]
        [Column("TimeTaken_ms")]
        public double TimeTaken_ms { get; set; }

        // Navigation property
        [ForeignKey("RoundID")]
        public virtual TSPRound? Round { get; set; }
    }
}
