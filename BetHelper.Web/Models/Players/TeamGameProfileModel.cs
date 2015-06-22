using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BetHelper.Web.Models.Players
{
    public class TeamGameProfileModel
    {


       
        public int TeamGameProfileId { get; set; }
        
        public char OwnGoals { get; set; }
        public char OthersGoals { get; set; }
        
        public char MatchSituation { get; set; }
        public char ResultOutcomeLetter { get; set; }
        public string RivalName { get; set; }                   
        public int MissedPen { get; set; }
        public int SavedPen { get; set; }
       
        public int Goallines { get; set; }
        public bool IsSelected { get; set; }
       
        public string PlayersMatchSchema { get; set; }
        public string GameDateDay { get; set; }
        public string GameDateMonth { get; set; }
        public string NewManager { get; set; }
        //public string ShotOnGoal { get; set; }
         public int AllShotsPlaced { get; set; }
        public int ShotsOnTargetPlaced { get; set; }
        public int AllShotsReceived { get; set; }
        public int CurrentFormLevel { get; set; }
        public int CurrentRivalFormLevel { get; set; }
        public string CurrentFormDetails { get; set; }
        public string CurrentFormRivalDetails { get; set; }
        public bool IsMileStone { get; set; }
        public int ShotsOnTargetReceived{ get; set; }
        public int SH { get; set; }
        public bool VisibilityMode { get; set; }
        public string WoodWorks { get; set; }
        public ICollection<RedCardEventModel> RedCardEvents { get; set; }
        public ICollection<string> PlayersIds { get; set; }
        
        public static Expression<Func<TeamGameProfile, TeamGameProfileModel>> FromProfile
        {
            get
            {
                return
                    profile =>
                    new TeamGameProfileModel
                        {
                            TeamGameProfileId = profile.TeamGameProfileId,
                            OwnGoals = profile.Result[0],
                            OthersGoals=profile.Result[2],
                            MatchSituation = profile.MatchSituation.ToString()[0],
                            ResultOutcomeLetter = profile.ResultOutcomeLetter.ToString()[0],
                            //changed to Nickname
                            IsSelected=false,
                            //changed to Nickname in the db
                            RivalName = profile.RivalName,
                            MissedPen = profile.MissedPen,
                            SavedPen = profile.SavedPen,
                            Goallines = profile.Goallines,
                            WoodWorks=profile.WoodWorks,
                            VisibilityMode=false,
                            NewManager=profile.NewManager,
                             AllShotsPlaced=profile.AllShotsPlaced,
                              AllShotsReceived=profile.AllShotsReceived,
                               ShotsOnTargetPlaced=profile.ShotsOnGoalPlaced,
                               ShotsOnTargetReceived=profile.ShotsOnGoalReceived,
                               CurrentFormLevel=profile.CurrentFormLevel,
                               CurrentRivalFormLevel=profile.Game.TeamGameProfiles.FirstOrDefault(x=>x.TeamName!=profile.TeamName).CurrentFormLevel,
                               IsMileStone=profile.IsMileStone,
                               CurrentFormDetails=profile.CurrentFormDetails,
                            CurrentFormRivalDetails = profile.Game.TeamGameProfiles.FirstOrDefault(x => x.TeamName != profile.TeamName).CurrentFormDetails,
                            GameDateDay = profile.Game.GameDate.Day.ToString(),
                            GameDateMonth =  profile.Game.GameDate.Month.ToString(),
                            SH=profile.SH,
                            PlayersMatchSchema = profile.PlayersMatchSchema,                                                                                                              
                            RedCardEvents = profile.RedCardEvents.AsQueryable()
                            .Select(RedCardEventModel.FromRedCardEvent).ToArray(),
                            PlayersIds = profile.Activities.Where(x => !x.MinutesPlayes.StartsWith("^") && !x.MinutesPlayes.StartsWith("v")
                                && !x.MinutesPlayes.StartsWith("N") && !x.MinutesPlayes.StartsWith("r")).Select(x=>x.PlayerId).ToArray()
                        };
            }
        }

    }
}

 