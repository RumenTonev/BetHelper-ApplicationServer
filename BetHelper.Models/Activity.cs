using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Models
{
    public class Activity
    {
        
        public Activity()
        {           
            
        }
       
        public int ActivityId { get; set; }
         public int AllAssists { get; set; }
         public int Goals { get; set; }
         public int GoalAssists { get; set; }
         public bool IsInjSub { get; set; }
         public string SubResult { get; set; }
         public  RedCard RedCard { get; set; }
        //TODO:Should set Minutesplaye on miising footballer injured,bannedd,nac teams,family
        public string MinutesPlayes { get; set; }
        public int AllShots { get; set; }
        public int Bars { get; set; }
        public int GoalLines { get; set; }
        public string PlayerCurrentNumber { get; set; }
        public string PlayerCurrentPosition { get; set; }
        public int ProfileId { get; set; }
        public string PlayerId{ get; set; }
        
        [ForeignKey("PlayerId")] 
        public virtual Player Player { get; set; }
        [ForeignKey("ProfileId")]
        public virtual TeamGameProfile Profile { get; set; }

    
    }
    
}
