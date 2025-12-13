using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.EightQueens
{
    [Table("EightQueens_AlgoTimes")]
    public class EQPAlgoTime
    {
        [Key]
        [Column("TimeID")]
        public int TimeID { get; set; }

        [Required]
        [Column("DateExecuted")]
        public string DateExecuted { get; set; } = string.Empty;

        [Required]
        [Column("AlgorithmType")]
        public string AlgorithmType { get; set; } = string.Empty;

        [Required]
        [Column("TimeTaken_ms")]
        public double TimeTaken_ms { get; set; }

        [Required]
        [Column("RoundNumber")]
        public int RoundNumber { get; set; }
    }
}
