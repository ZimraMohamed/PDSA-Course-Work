using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PDSA.API.Data.Models.TSP;
using PDSA.API.Data.Models.EightQueens;

namespace PDSA.API.Data.Models
{
    [Table("Players")]
    public class Player
    {
        [Key]
        [Column("PlayerID")]
        public int PlayerID { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<TSPRound> TSPRounds { get; set; } = new List<TSPRound>();
        public virtual ICollection<EQPSolution> EQPSolutions { get; set; } = new List<EQPSolution>();
    }
}
