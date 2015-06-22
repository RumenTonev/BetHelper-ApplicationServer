using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
    public class RedCard
    {        
        [Key()]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ActivityId { get; set; }
        public string munitesReceiving { get; set; }
        public string resultReceiving { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
