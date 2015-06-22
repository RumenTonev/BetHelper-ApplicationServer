using BetHelper.Data;
using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using BetHelper.Web.Models.Profiles;

using BetHelper.Web.Models.Games;

namespace BetHelper.Web.Controllers
{
    public class ProfilesController : BaseApiController
    {
        public ProfilesController()
            : this(new BetHelperData())
        {
        }

        public ProfilesController(IBetHelperData data)
            : base(data)
        {
        }
        //on client-side click sav values DB
        [HttpPut]
        public IHttpActionResult UpdateMilestone(int id)
        {
            var profile = this.Data.TeamGameProfiles.Find(id);
            if (profile == null)
            {
                return this.BadRequest("no such profile.");
            }
            profile.IsMileStone = !profile.IsMileStone;
            this.Data.SaveChanges();
            return this.Ok();
        }
        [HttpPost]
        public IHttpActionResult UpdateDuplicateProfile(ProfileModelDuplicateNames profileModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            var firstPlayer = this.Data.Players.Find(profileModel.FirstPlayerId);

            firstPlayer.PlayerName = profileModel.FirstPlayerName;
            if (profileModel.SecondPlayerId != null)
            {
                var secondPlayer = this.Data.Players.Find(profileModel.SecondPlayerId);
                secondPlayer.PlayerName = profileModel.SecondPlayerName;
            }
            this.Data.SaveChanges();
            return this.Ok();
        }


        [HttpPost]
        public IHttpActionResult UpdateUndefinedProfile(ProfileModelUndefinedActivity profileModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            var profile = this.Data.TeamGameProfiles.Find(profileModel.ProfileId);
            var player = this.Data.Players.Find(profileModel.PlayerId);
            player.PlayerName = profileModel.PlayerNewName;

            var possibleDFA = this.Data.DateFirstAppearances.All().FirstOrDefault(x => x.PlayerId == player.PlayerId && x.TeamName == profile.TeamName);
            if (possibleDFA == null)
            {
                player.DateFirstAppearance = DateTime.Now;
                var DFA = new DateFirstAppearance()
                {
                    DateTimeStart = DateTime.Now,
                    Player = player,
                    TeamName = profile.TeamName,
                    PlayerId = player.PlayerId
                };
                this.Data.DateFirstAppearances.Add(DFA);
            }
            else
            {
                player.DateFirstAppearance = possibleDFA.DateTimeStart;

            }
            //merge of activities
            var undefinedActivivity = profile.Activities.FirstOrDefault(x => x.PlayerId == profileModel.UndefinedId);
            var actualActivity = profile.Activities.FirstOrDefault(x => x.PlayerId == profileModel.PlayerId);
            actualActivity.MinutesPlayes = undefinedActivivity.MinutesPlayes;
            actualActivity.AllAssists = undefinedActivivity.AllAssists;
            actualActivity.AllShots = undefinedActivivity.AllShots;
            actualActivity.Bars = undefinedActivivity.Bars;
            actualActivity.GoalAssists = undefinedActivivity.GoalAssists;
            actualActivity.GoalLines = undefinedActivivity.GoalLines;
            actualActivity.Goals = undefinedActivivity.Goals;
            actualActivity.IsInjSub = undefinedActivivity.IsInjSub;
            actualActivity.SubResult = undefinedActivivity.SubResult;
            //check redcards
            var rc = this.Data.RedCards.All().FirstOrDefault(x => x.ActivityId == undefinedActivivity.ActivityId);
            if (rc != null)
            {
                var redCardNew = new RedCard()
                {
                    Activity = actualActivity,
                    ActivityId = actualActivity.ActivityId,
                    munitesReceiving = rc.munitesReceiving,
                    resultReceiving = rc.resultReceiving
                };
                this.Data.RedCards.Add(redCardNew);
                this.Data.RedCards.Delete(rc);
                this.Data.SaveChanges();
            }

            this.Data.Players.Delete(profileModel.UndefinedId);
            this.Data.SaveChanges();

            return this.Ok();
        }

        [HttpPut]

        public IHttpActionResult UpdateCurrentFormLevel(int id, string newContent)
        {
            var profile = this.Data.TeamGameProfiles.Find(id);
            if (profile == null)
            {
                return this.BadRequest("No such profile.");
            }
            profile.CurrentFormLevel = Int32.Parse(newContent);
            this.Data.SaveChanges();
            return this.Ok();
        }

        [HttpPut]
        public IHttpActionResult UpdateCurrentFormDetails(int id, string newContent)
        {
            var profile = this.Data.TeamGameProfiles.Find(id);
            if (profile == null)
            {
                return this.BadRequest("No such profile.");
            }
            profile.CurrentFormDetails = newContent;
            this.Data.SaveChanges();
            return this.Ok();
        }

        [HttpPut]
        public IHttpActionResult UpdateManager(ProfileModelManagerChange profileObject)
        {
            var profile = this.Data.TeamGameProfiles.Find(profileObject.ProfileId);
            if (profile == null)
            {
                return this.BadRequest("No such profile.");
            }
            profile.NewManager = profileObject.NewManager;
            this.Data.SaveChanges();
            return this.Ok();
        }

        [HttpPut]
        public IHttpActionResult UpdateGameStatus(NewGameStatusModel game)
        {
            var currGame = this.Data.Games.Find(game.GameId);
            if (currGame == null)
            {
                return this.BadRequest("The requested game was not found.");
            }
            var newDate = new DateTime(game.Year, game.Month, game.Day, game.Hour, game.Min, 0);
            currGame.GameDate = newDate;
            currGame.GameStatus = GameStatus.Fixture;
            this.Data.SaveChanges();
            return this.Ok();
        }
    }
}
