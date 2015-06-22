using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Net.Http;
using BetHelper.Web.Models.Games;

namespace BetHelper.Web.Hubs
{
    public class CurrentGameInfoHub : Hub
    {
        public static void PushCurrentGameProfile(GamePushingProfile gameObject)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<CurrentGameInfoHub>();
            if (hubContext != null)
            {               
                hubContext.Clients.All.notifyLinkMainPage(gameObject);
                hubContext.Clients.All.pushTeamData(gameObject);
            }
        }
    }
}