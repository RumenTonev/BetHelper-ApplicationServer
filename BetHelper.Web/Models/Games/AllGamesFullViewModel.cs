using BetHelper.Data;
using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BetHelper.Web.Models.Teams;

namespace BetHelper.Web.Models.Games
{
    public class SignleGameFullViewModel
    {

        BetHelperContext ctx;
        BetHelperData dat;

        //TODO da podavam direktno obekt Game
        //TODO da gi zapi6a pravilno v BAZATATA game .Hostyanem i t t t
        public SignleGameFullViewModel(Game game)
        {

            this.GameId = game.GameId;
           this.ctx=new BetHelperContext();
            this.dat=new BetHelperData(ctx);
            this.HostModel = new TeamRealTimeView(this.dat.Teams.Find(game.HostName.Trim()));
            this.GuestModel = new TeamRealTimeView(this.dat.Teams.Find(game.GuestName.Trim()));
            this.DateForGame = game.GameDate;
            this.Status = game.GameStatus.ToString();
           
            
           
        }
        

        public string GameId { get; set; }
        //public string HostName { get; set; }
//public string GuestName { get; set; }
        public string Status { get; set; }
        public DateTime DateForGame { get; set; }

        public TeamRealTimeView HostModel { get; set; }
        public TeamRealTimeView GuestModel { get; set; }
    }
}