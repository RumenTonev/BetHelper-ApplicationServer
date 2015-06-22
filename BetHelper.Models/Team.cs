using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
    public class Team
    {
        public Team()
        {
            this.TeamGameProfiles = new HashSet<TeamGameProfile>();
            this.Players = new HashSet<Player>();
            this.Loans = new HashSet<Loan>();
            this.ManagerChanges = new HashSet<ManagerChange>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [Required]
        public string Name { get; set; }
        public string NickName { get; set; }
        public virtual ICollection<TeamGameProfile> TeamGameProfiles { get; set; }
        public virtual ICollection<ManagerChange> ManagerChanges { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
        public virtual ICollection<Player> Players { get; set; }
        [Required]
        public string DivisionID { get; set; }
        [ForeignKey("DivisionID")]
        public virtual Division Division { get; set; } 
    }
}
