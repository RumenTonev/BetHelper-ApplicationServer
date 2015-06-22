using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
   public  class RedCardProfileEvent
    {

        [Key]
        [Required]
        public int RedCardProfileEventId { get; set; }
        public int PeriodInMinutes { get; set; }
        public int CountCurrentPlayersDifference { get; set; }
        public string ResultReceiving { get; set; }
        public RedCardSituation RedCardPlayStatus { get; set; }
        public int TeamGameProfileId { get; set; }
        public virtual TeamGameProfile TeamGameProfile { get; set; }
    }
}
