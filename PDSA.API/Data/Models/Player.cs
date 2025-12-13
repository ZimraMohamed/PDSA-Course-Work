using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PDSA.API.Data.Models.TSP;
using PDSA.API.Data.Models.EightQueens;
using PDSA.API.Data.Models.TrafficSimulation;
using PDSA.API.Data.Models.TowerOfHanoi;

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
        public virtual ICollection<TrafficRound> TrafficRounds { get; set; } = new List<TrafficRound>();
        public virtual ICollection<HanoiRound> HanoiRounds { get; set; } = new List<HanoiRound>();
    }
}
