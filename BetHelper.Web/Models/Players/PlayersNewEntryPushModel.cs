using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetHelper.Web.Models.Players
{
    //Model used for New players pushing-Signalr connection-must be changed profiles count
    public class PlayersNewEntryPushModel
    {
        public PlayersNewEntryPushModel(int profilesCount)
        {
            this.activities = new EmptyActivity[profilesCount]; 
        }
         public string playerName { get; set; }
        public string playerNumber { get; set; }
           
        public string currentMinPlayes { get; set; }
        public  EmptyActivity [] activities{ get; set; }
        public DateTime GameDate { get; set; }
         
        

    }
}