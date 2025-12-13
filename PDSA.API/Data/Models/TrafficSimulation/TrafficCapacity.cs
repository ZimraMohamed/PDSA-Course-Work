using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TrafficSimulation
{
    [Table("TrafficSim_Capacities")]
    public class TrafficCapacity
    {
        [Key]
        [Column("CapacityID")]
        public int CapacityID { get; set; }

        [Required]
        [Column("RoundID")]
        public int RoundID { get; set; }

        [Required]
        [Column("RoadSegment")]
        public string RoadSegment { get; set; } = string.Empty;

        [Required]
        [Column("Capacity_VehPerMin")]
        public int Capacity_VehPerMin { get; set; }

        // Navigation property
        [ForeignKey("RoundID")]
        public virtual TrafficRound? Round { get; set; }
    }
}
