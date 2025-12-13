using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TowerOfHanoi
{
    [Table("Hanoi_AlgoTimes")]
    public class HanoiAlgoTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimeID { get; set; }

        [Required]
        public int RoundID { get; set; }

        [Required]
        public string AlgorithmName { get; set; } = string.Empty;

        [Required]
        public double TimeTaken_ms { get; set; }

        // Navigation property
        [ForeignKey("RoundID")]
        public HanoiRound? Round { get; set; }
    }
}
