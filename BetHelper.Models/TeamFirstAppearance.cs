using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
   public class TeamFirstAppearance
    {
        public int TeamFirstAppearanceId { get; set; }
        public string TeamName { get; set; }
        public string PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }
    }
}
