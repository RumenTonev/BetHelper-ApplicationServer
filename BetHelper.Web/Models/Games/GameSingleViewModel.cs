using BetHelper.Data;
using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BetHelper.Web.Models.Players;
using BetHelper.Web.Models.Teams;

namespace BetHelper.Web.Models.Games
{
    public class GameSingleViewModel
    {
 
        public GameSingleViewModel(Game game)
        {           
            this.GameId = game.GameId;    
            this.HostName = game.HostName;
            this.GuestName = game.GuestName;
            this.DateForGame = game.GameDate;
        }
        

        public string GameId { get; set; }
        public string HostName { get; set; }
        public string GuestName { get; set; }       
        public DateTime DateForGame { get; set; }

    }
}