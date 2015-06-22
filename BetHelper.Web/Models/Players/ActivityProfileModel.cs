using BetHelper.Data;
using BetHelper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BetHelper.Web.Models.Players
{
    public class ActivityProfileModel
    {
      

        public ActivityProfileModel(Activity activity)
        {
           // this.context = new BetHelperContext();
           // this.dat = new BetHelperData(context);
              this.ActivityId=activity.ActivityId;
                    this.AllAssists=activity.AllAssists;
                    this.Goals = activity.Goals;
                    this.GoalAssists = activity.GoalAssists;
                    this.MinutesPlayes = activity.MinutesPlayes;
                    this.AllShots = activity.AllShots;
                    this.Bars = activity.Bars;
                    this.GoalLines = activity.GoalLines;
                    this.PlayerCurrentPosition = activity.PlayerCurrentPosition;
                    this.ProfileId = activity.ProfileId;
                   // this.ActivityDate = activity.Profile.Game.GameDate;
                    this.PlayerId = activity.PlayerId;
                    this.GameStatus = activity.Profile.Game.GameStatus;
                    this.RedCard = (activity.RedCard == null) ? null : new RedCardModel(activity.RedCard);
            this.IsInjSub=activity.IsInjSub;
            this.SubResult=activity.SubResult;
            this.IsMileStone = activity.Profile.IsMileStone;
            this.PS = activity.Profile.SH;
        }

        

      

        public int ActivityId { get; set; }
        public int AllAssists { get; set; }
        public int Goals { get; set; }
        public int GoalAssists { get; set; }
        public RedCardModel RedCard { get; set; }
        //TODO:Should set Minutesplaye on miising footballer in
//jured,bannedd,nac teams,family
        public string MinutesPlayes { get; set; }
        public int AllShots { get; set; }
        public int Bars { get; set; }
        public int GoalLines { get; set; }
        public DateTime ActivityDate { get; set; }
        public GameStatus GameStatus { get; set; }
        public string PlayerCurrentPosition { get; set; }
        public int ProfileId { get; set; }
        public string PlayerId { get; set; }
        public bool IsInjSub { get; set; }
        public string SubResult{ get; set; }
        public bool IsMileStone { get; set; }
        public bool IsKeyMiss { get; set; }
        public int PS { get; set; }

     
        
    }
}