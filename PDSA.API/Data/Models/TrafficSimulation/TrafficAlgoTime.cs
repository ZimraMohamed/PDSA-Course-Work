using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TrafficSimulation
{
    [Table("TrafficSim_AlgoTimes")]
    public class TrafficAlgoTime
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
        public virtual TrafficRound? Round { get; set; }
    }
}
