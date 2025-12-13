using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TSP
{
    [Table("TSP_Distances")]
    public class TSPDistance
    {
        [Key]
        [Column("DistanceID")]
        public int DistanceID { get; set; }

        [Required]
        [Column("RoundID")]
        public int RoundID { get; set; }

        [Required]
        [Column("City_A")]
        public string City_A { get; set; } = string.Empty;

        [Required]
        [Column("City_B")]
        public string City_B { get; set; } = string.Empty;

        [Required]
        [Column("Distance_km")]
        public int Distance_km { get; set; }

        // Navigation property
        [ForeignKey("RoundID")]
        public virtual TSPRound? Round { get; set; }
    }
}
