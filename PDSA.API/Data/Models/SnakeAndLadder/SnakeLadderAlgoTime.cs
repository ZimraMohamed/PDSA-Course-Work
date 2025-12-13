using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.SnakeAndLadder
{
    [Table("SnakeLadder_AlgoTimes")]
    public class SnakeLadderAlgoTime
    {
        [Key]
        public int TimeID { get; set; }

        [Required]
        public int RoundID { get; set; }

        [Required]
        [MaxLength(100)]
        public string AlgorithmName { get; set; } = string.Empty;

        [Required]
        public double TimeTaken_ms { get; set; }

        // Navigation property
        [ForeignKey("RoundID")]
        public virtual SnakeLadderRound? Round { get; set; }
    }
}
