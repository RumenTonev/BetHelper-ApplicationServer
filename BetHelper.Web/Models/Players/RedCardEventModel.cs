using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BetHelper.Web.Models.Players
{
    public class RedCardEventModel
    {
        public static Expression<Func<RedCardProfileEvent, RedCardEventModel>> FromRedCardEvent
        {
            get
            {
                return profileEvent => new RedCardEventModel
                {

                    RedCardProfileEventId=profileEvent.RedCardProfileEventId,
                    PeriodInMinutes=profileEvent.PeriodInMinutes,
                    CountCurrentPlayersDifference=profileEvent.CountCurrentPlayersDifference,
                    ResultReceiving=profileEvent.ResultReceiving,
                    RedCardPlayStatus=profileEvent.RedCardPlayStatus.ToString(),
                     TeamGameProfileId=profileEvent.TeamGameProfileId
                   
                };
            }
        }


        public int RedCardProfileEventId { get; set; }
        public int PeriodInMinutes { get; set; }
        public int CountCurrentPlayersDifference { get; set; }
        public string ResultReceiving { get; set; }

        public string RedCardPlayStatus { get; set; }


        public int TeamGameProfileId { get; set; }


       

    }
}