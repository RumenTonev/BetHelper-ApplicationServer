using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
    public class ManagerChange
    {
        public int ManagerChangeId { get; set; }
        public string NewManagerName { get; set; }

        public DateTime ManagerChangeDate { get; set; }

        public string TeamName { get; set; }
        [ForeignKey("TeamName")]
        public virtual Team Team { get; set; }
    }
    
}
