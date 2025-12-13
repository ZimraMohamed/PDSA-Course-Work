using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TrafficSimulation
{
    [Table("TrafficSim_Rounds")]
    public class TrafficRound
    {
        [Key]
        [Column("RoundID")]
        public int RoundID { get; set; }

        [Required]
        [Column("PlayerID")]
        public int PlayerID { get; set; }

        [Required]
        [Column("CorrectMaxFlow")]
        public double CorrectMaxFlow { get; set; }

        [Required]
        [Column("DatePlayed")]
        public string DatePlayed { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }

        public virtual ICollection<TrafficCapacity> Capacities { get; set; } = new List<TrafficCapacity>();
        public virtual ICollection<TrafficAlgoTime> AlgorithmTimes { get; set; } = new List<TrafficAlgoTime>();
    }
}
