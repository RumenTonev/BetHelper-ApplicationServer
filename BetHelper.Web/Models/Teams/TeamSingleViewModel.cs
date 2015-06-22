using BetHelper.Data;
using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetHelper.Web.Models.Players;
using System.Data.Entity;

namespace BetHelper.Web.Models.Teams
{

    //TODO: da podavam direktno obekt
    public class TeamSingleViewModel
    {   
             private BetHelperContext context;
    private BetHelperData dat;
        
        public TeamSingleViewModel(Team team)
        {
            
            this.Name = team.Name;

            this.context = new BetHelperContext();
            this.dat = new BetHelperData(context);
            //ACTUAL
            // this.TeamGameProfiles = team.TeamGameProfiles.Where(y => y.Game.GameDate.Date < DateTime.Today 
            //DEMO
            this.TeamGameProfiles = team.TeamGameProfiles.Where(y => y.Game.GameDate.Date < new DateTime(2015, 4, 6, 1, 0, 0)
                && y.Game.GameStatus == GameStatus.Archived).OrderBy(x => x.Game.GameDate).AsQueryable()
                .Select(TeamGameProfileModel.FromProfile).ToArray();
            this.Players = GetProfiles(team.Name);
            //ACTUAL
            //  this.CurrentProfile = team.TeamGameProfiles.Where(y => y.Game.GameDate.Date >= DateTime.Today).OrderBy(x => x.Game.GameDate).AsQueryable()
            //DEMO
            this.CurrentProfile = team.TeamGameProfiles.Where(y => y.Game.GameDate.Date >= new DateTime(2015, 4, 6, 1, 0, 0)).OrderBy(x => x.Game.GameDate).AsQueryable()
       .Select(TeamGameProfileModel.FromProfile).First();
           
            
           
        }
        
        public string Name { get; set; }
        
        
        public  ICollection<TeamGameProfileModel> TeamGameProfiles { get; set; }
        public TeamGameProfileModel CurrentProfile { get; set; }
       
        public  ICollection<PlayerProfileModel> Players { get; set; }
        public PlayersNewEntryPushModel[] NewPlayers { get; set; }
        private  PlayerProfileModel[] GetProfiles(string teamName)
        {
            var result1 = this.dat.Players.All().Where(x => x.DateFirstAppearances.Select(y => y.TeamName).
                Contains(teamName));
            var result2=result1.AsQueryable().OrderBy(z=>z.SortIndex).ThenBy(x => x.DateFirstAppearances.FirstOrDefault(y => y.TeamName == teamName).DateTimeStart).ToList()
            .Select(x => new PlayerProfileModel(teamName, x)).ToArray();
            //var result2=result1.OrderBy(x => x.DateFirstAppearances.FirstOrDefault(y => y.TeamName == teamName).DateTimeStart).ToList()
                 //.Select(x => new PlayerProfileModel(teamName, x)).ToArray();
            return result2;
        }
    }
}
 