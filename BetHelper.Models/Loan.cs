using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
   public class Loan
    {
        public int LoanId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerParentClubId { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string TeamName { get; set; }
        [ForeignKey("TeamName")]
        public virtual Team Team { get; set; }
    }
}
