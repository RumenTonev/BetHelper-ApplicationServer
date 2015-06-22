using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetHelper.Web.Models.Players;

namespace BetHelper.Web.Models.Games
{
    public class GamePushingProfile
    {
       // public string Id { get; set; }

        public string GameId { get; set; }


        public SignaRPushModel[] CurrentPassedHomeOldPlayers { get; set; }

        public SignaRPushModel[] CurrentPassedAwayOldPlayers { get; set; }
        public PlayersNewEntryPushModel[] CurrentPassedHomeNewPlayers { get; set; }
          public PlayersNewEntryPushModel[] CurrentPassedAwayNewPlayers { get; set; }


    }
}