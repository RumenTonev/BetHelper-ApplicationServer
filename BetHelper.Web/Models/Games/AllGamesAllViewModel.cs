using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BetHelper.Web.Models.Games
{
    public class AllGamesAllViewModel
    {
        public static Expression<Func<Game, AllGamesAllViewModel>> FromGame
        {
            get
            {
                return gameModel => new AllGamesAllViewModel
                {
                    GameId = gameModel.GameId,
                    DateForGame =gameModel.GameDate,
                   HostName = gameModel.HostName,
                    GuestName = gameModel.GuestName,
                    Status=gameModel.GameStatus.ToString()
                };
            }
        }

        public string GameId { get; set; }
        public string HostName { get; set; }
        public string GuestName { get; set; }
        public string Status { get; set; }

        public DateTime DateForGame { get; set; }
    }
}