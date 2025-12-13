using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.TowerOfHanoi
{
    [Table("Hanoi_Rounds")]
    public class HanoiRound
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoundID { get; set; }

        [Required]
        public int PlayerID { get; set; }

        [Required]
        public int NumDisks_N { get; set; }

        [Required]
        public int NumPegs { get; set; }

        [Required]
        public int CorrectMoves_Count { get; set; }

        [Required]
        public string CorrectMoves_Sequence { get; set; } = string.Empty;

        [Required]
        public string DatePlayed { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("PlayerID")]
        public Player? Player { get; set; }

        public ICollection<HanoiAlgoTime> AlgorithmTimes { get; set; } = new List<HanoiAlgoTime>();
    }
}
