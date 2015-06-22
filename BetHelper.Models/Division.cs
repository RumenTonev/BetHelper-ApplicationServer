using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BetHelper.Models
{
    public class Division
    {

          public Division()
        {
            this.Teams = new HashSet<Team>();
            this.Games = new HashSet<Game>();
           
        }

          [DatabaseGenerated(DatabaseGeneratedOption.None)]
          [Key]
          [Required]
        public string Name { get; set; }
          public string Country { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}
