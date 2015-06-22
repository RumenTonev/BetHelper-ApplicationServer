using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
    public class TeamGameProfile
    {
        public TeamGameProfile()
        {
            this.Participiants = new HashSet<Player>();
            this.Activities = new HashSet<Activity>();
            this.RedCardEvents = new HashSet<RedCardProfileEvent>();
        }
        public int TeamGameProfileId { get; set; }
        public string Result { get; set; }
        [Required]
        public MatchSituation MatchSituation { get; set; }
        public ResultOutcome ResultOutcomeLetter { get; set; }
        public string RivalName { get; set; }
        public virtual ICollection<Player> Participiants { get; set; }
        public virtual ICollection<RedCardProfileEvent> RedCardEvents { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public int CurrentFormLevel { get; set; }
        public int CurrentRivalFormLevel { get; set; }
        public string CurrentFormDetails { get; set; }
        public string CurrentFormRivalDetails { get; set; }
        public bool IsMileStone { get; set; }
        public int MissedPen { get; set; }
        public int SavedPen { get; set; }
        public int Bars { get; set; }
        public int Goallines { get; set; }
        public string ShotOnGoal { get; set; }
        public string NewManager { get; set; }
        public int ShotOffTargetPlaced { get; set; }
        public int ShotBlockedPlaced { get; set; }
        public int ShotBlockedReceived { get; set; }
        public string WoodWorks { get; set; }
        public int ShotOffTargetReceived { get; set; }
        public int AllShotsPlaced { get; set; }
        public int AllShotsReceived { get; set; }
        public int ShotsOnGoalPlaced { get; set; }
        public int ShotsOnGoalReceived { get; set; }
        public int AllShots { get; set; }
        public string PlayersMatchSchema { get; set; }
        public int SH { get; set; }
        public string TeamName { get; set; }
        [ForeignKey("TeamName")]
        public virtual Team Team { get; set; }
        public string GameId { get; set; }
        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }
    }
}
