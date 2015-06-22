using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BetHelper.Models
{
    public class Game
    {
        public Game()
        {           
            this.TeamGameProfiles = new HashSet<TeamGameProfile>();
            
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        [Required]
        public string GameId { get; set; }

        public string ScoresId { get; set; }
        public DateTime GameDate { get; set; }
        public GameStatus GameStatus { get; set; }
        public string HostName { get; set; }
        public string GuestName { get; set; }
        //TODO:add GuestName
        public string StartingHour { get; set; }
        public virtual ICollection<TeamGameProfile> TeamGameProfiles { get; set; }
        [Required]
        public string DivisionID { get; set; }
        [ForeignKey("DivisionID")]
        public virtual Division Division { get; set; }
    
    }
}
