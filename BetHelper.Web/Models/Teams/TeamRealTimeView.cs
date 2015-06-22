using BetHelper.Data;
using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using BetHelper.Web.Models.Players;

namespace BetHelper.Web.Models.Teams
{
    public class TeamRealTimeView
    {
           private BetHelperContext context;
    private BetHelperData dat;
        
        public TeamRealTimeView(Team team)
        {
           
            this.Name = team.Name;
            this.Nick = team.NickName;
            this.context = new BetHelperContext();
            this.dat = new BetHelperData(context);
            this.TeamGameProfiles = team.TeamGameProfiles.Where(y => y.Game.GameDate.Date < DateTime.Today 
                && y.Game.GameStatus == GameStatus.Archived).OrderByDescending(x => x.Game.GameDate).Take(10).OrderBy(x => x.Game.GameDate).AsQueryable()
                .Select(TeamShortModel.FromProfile).ToArray();
            this.Players = GetProfiles(team.Name);
            //ACTUAL
            // this.CurrentProfile = team.TeamGameProfiles.Where(y => y.Game.GameDate.Date >= DateTime.Today).OrderBy(x => x.Game.GameDate).AsQueryable()
            // .Select(TeamShortModel.FromProfile).First();
            //DEMO
            this.CurrentProfile = team.TeamGameProfiles.Where(y => y.Game.GameDate.Date >= new DateTime(2015, 4, 6, 1, 0, 0)).OrderBy(x => x.Game.GameDate).AsQueryable()
               .Select(TeamShortModel.FromProfile).First();
            
           
        }
        
        public string Name { get; set; }
        
        
        public  ICollection<TeamShortModel> TeamGameProfiles { get; set; }
        public TeamShortModel CurrentProfile { get; set; }
        public string Nick { get; set; }
       
        public  ICollection<PlayerShortModel> Players { get; set; }
        public PlayersNewEntryPushModel[] NewPlayers { get; set; }
        private  PlayerShortModel[] GetProfiles(string teamName)
        {
            var result1 = this.dat.Players.All().Where(x => x.DateFirstAppearances.Select(y => y.TeamName).
                Contains(teamName));
            var result2=result1.AsQueryable().OrderBy(z=>z.SortIndex).ThenBy(x => x.DateFirstAppearances.FirstOrDefault(y => y.TeamName == teamName).DateTimeStart).ToList()
            .Select(x => new PlayerShortModel(teamName, x)).ToArray();
            var finalResult = result2.Where(x => x.Activities.FirstOrDefault(y => y.MinutesPlayes.StartsWith("t")
                || Regex.IsMatch(y.MinutesPlayes, "^[0-9]*$") || y.GoalAssists != 0 || y.Goals != 0 ||
                (y.MinutesPlayes.StartsWith("^") && Int32.Parse(y.MinutesPlayes.Substring(1)) > 45) || (y.MinutesPlayes.StartsWith("v") && Int32.Parse(y.MinutesPlayes.Substring(1)) > 45)) != null).ToArray();
            //var result2=result1.OrderBy(x => x.DateFirstAppearances.FirstOrDefault(y => y.TeamName == teamName).DateTimeStart).ToList()
                 //.Select(x => new PlayerProfileModel(teamName, x)).ToArray();
            return finalResult;
        }
    }
}