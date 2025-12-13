using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDSA.API.Data.Models.EightQueens
{
    [Table("EightQueens_Solutions")]
    public class EQPSolution
    {
        [Key]
        [Column("SolutionID")]
        public int SolutionID { get; set; }

        [Column("PlayerID")]
        public int? PlayerID { get; set; }

        [Column("DateFound")]
        public string? DateFound { get; set; }

        [Required]
        [Column("Solution_Text")]
        public string Solution_Text { get; set; } = string.Empty;

        [Required]
        [Column("IsFound")]
        public bool IsFound { get; set; } = false;

        // Navigation property
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }
    }
}
