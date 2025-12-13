using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.SnakeAndLadder
{
    [Table("SnakeLadder_BoardConfig")]
    public class SnakeLadderBoardConfig
    {
        [Key]
        public int ConfigID { get; set; }

        [Required]
        public int RoundID { get; set; }

        [Required]
        [MaxLength(10)]
        public string FeatureType { get; set; } = string.Empty; // 'Ladder' or 'Snake'

        [Required]
        public int Start_Cell { get; set; }

        [Required]
        public int End_Cell { get; set; }

        // Navigation property
        [ForeignKey("RoundID")]
        public virtual SnakeLadderRound? Round { get; set; }
    }
}
