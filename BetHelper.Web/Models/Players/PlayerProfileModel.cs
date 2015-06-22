using BetHelper.Data;
using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity;
namespace BetHelper.Web.Models.Players
{
    public class PlayerProfileModel
    {

        
        BetHelperContext context;
        BetHelperData dat;
        
           
   public PlayerProfileModel(PlayersNewEntryPushModel model)
        { }
        public PlayerProfileModel(string teamName,Player player)
        {
             this.context = new BetHelperContext();
            this.dat = new BetHelperData(context);

            int todayYear = DateTime.Now.Year;
            int todayMonth = DateTime.Now.Month;
            int todayDay = DateTime.Now.Day;
                   this.PlayerId = player.PlayerId;
                    this.PlayerName = FormatName( player.PlayerName);
                   this. PlayerNumber = player.PlayerNumber;
                   this.Position = player.Position;
                   this.DateFirstAppearance = player.DateFirstAppearances.Where(x => x.TeamName == teamName).First().DateTimeStart;

                   this.Activities = dat.Activities.All().Where(x => x.PlayerId == player.PlayerId).Where(x => x.Profile.TeamName == teamName
                       //ACTUAL
                       // && x.Profile.Game.GameDate < new DateTime(todayYear, todayMonth, todayDay, 1, 0, 0)
                       //DEMO
                       && x.Profile.Game.GameDate < new DateTime(2015, 4, 6, 1, 0, 0)
                        && x.Profile.Game.GameStatus != GameStatus.PostPoned).OrderBy(y => y.Profile.Game.GameDate).AsQueryable().Include(x => x.RedCard).ToList()
                .Select(x => new ActivityProfileModel(x)).ToArray();
                   //THE OLD MODEL
                   // this.Activities = dat.Activities.All().Where(x => x.PlayerId == player.PlayerId).Where(x => x.Profile.TeamName == teamName).AsQueryable().Include("RedCard").ToList()
                   // .Select(x => new ActivityProfileModel(x)).OrderBy(y => y.ActivityDate).ToArray();
                   //ACTUAL
                   //this.CurrentActivity = player.PlayerActivities.Where(x => x.Profile.TeamName == teamName && x.Profile.Game.GameDate >= new DateTime(todayYear, todayMonth, todayDay, 1, 0, 0)).OrderBy(x => x.Profile.Game.GameDate)
                   //.Select(x => new ActivityProfileModel(x)).First();
                   //DEMO
                   this.CurrentActivity = player.PlayerActivities.Where(x => x.Profile.TeamName == teamName && x.Profile.Game.GameDate >= new DateTime(2015, 4, 6, 1, 0, 0)).OrderBy(x => x.Profile.Game.GameDate)
            .Select(x => new ActivityProfileModel(x)).First();
        }

        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Position { get; set; }
        public string PlayerNumber { get; set; }
        public string CurrentGameMinPlayes { get; set; }
        public DateTime DateFirstAppearance { get; set; }
        //new Property for currentActivity
        public ActivityProfileModel CurrentActivity { get; set; }
        public ICollection<ActivityProfileModel> Activities { get; set; }
        private static string FormatName(string name)
        {
            string[] splitters = { " ", "+" };

            string[] resultArray = name.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

            string result = resultArray[0];
            if (resultArray.Length > 1)
            {
                result = resultArray[0] + " " + resultArray[1];
            }
            return result;
        }
    }
}