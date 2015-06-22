using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BetHelper.Web.Models.Players
{
    public class RedCardModel
    {

        public RedCardModel(RedCard redCard)
        {

            this.ActivityId = redCard.ActivityId;
            this.MunitesReceiving = redCard.munitesReceiving;
            this.ResultReceiving = redCard.resultReceiving;
        }
        
        
      
        public int ActivityId { get; set; }
        public string MunitesReceiving { get; set; }
        public string ResultReceiving { get; set; }
    }
}