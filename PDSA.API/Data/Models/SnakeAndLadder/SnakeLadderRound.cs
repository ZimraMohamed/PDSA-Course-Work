using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.SnakeAndLadder
{
    [Table("SnakeLadder_Rounds")]
    public class SnakeLadderRound
    {
        [Key]
        public int RoundID { get; set; }

        [Required]
        public int PlayerID { get; set; }

        [Required]
        public int BoardSize_N { get; set; }

        [Required]
        public int NumLadders { get; set; }

        [Required]
        public int NumSnakes { get; set; }

        [Required]
        public int CorrectMinThrows { get; set; }

        [Required]
        public string DatePlayed { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        // Navigation properties
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }

        public virtual ICollection<SnakeLadderBoardConfig> BoardConfigs { get; set; } = new List<SnakeLadderBoardConfig>();
        public virtual ICollection<SnakeLadderAlgoTime> AlgorithmTimes { get; set; } = new List<SnakeLadderAlgoTime>();
    }
}
