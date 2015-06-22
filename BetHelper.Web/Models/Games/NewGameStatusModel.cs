using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetHelper.Web.Models.Games
{
    public class NewGameStatusModel
    {
        public string GameId { get; set; }


        public int Year { get; set; }

        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Min { get; set; }
    }
}
