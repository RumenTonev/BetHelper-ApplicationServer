using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BetHelper.Models
{
    public class Player
    {
        public static int numberOfAddition = 0;
        public Player()
        {
            this.TeamGameProfiles = new HashSet<TeamGameProfile>();
            this.PlayerActivities = new HashSet<Activity>();
            this.DateFirstAppearances = new HashSet<DateFirstAppearance>();
            this.TeamFirstAppearances = new HashSet<TeamFirstAppearance>();

        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [Required]
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNumber { get; set; }
        public byte SortIndex { get; set; }
        public DateTime? DateFirstAppearance { get; set; }
        public virtual ICollection<TeamGameProfile> TeamGameProfiles { get; set; }
        public virtual ICollection<Activity> PlayerActivities { get; set; }
        public virtual ICollection<DateFirstAppearance> DateFirstAppearances { get; set; }
        public virtual ICollection<TeamFirstAppearance> TeamFirstAppearances { get; set; }
        public string Position { get; set; }
        public string TeamName { get; set; }
        [ForeignKey("TeamName")]
        public virtual Team Team { get; set; }
    }
}
