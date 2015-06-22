using BetHelper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using BetHelper.Models;
using BetHelper.Web.Models.Teams;
using System.Web.Http.Cors;
using System.Web.Caching;

namespace BetHelper.Web.Controllers
{
    
    public class TeamsController : BaseApiController
    {       
       public TeamsController()
            : this(new BetHelperData())
        {
        }

        public TeamsController(IBetHelperData data)
            : base(data)
        {
        }
   
        [HttpGet]
        public IHttpActionResult GetAllTeams()
        {
            var teams = this.Data.Teams.All().AsQueryable().ToList().Select(x => new TeamNickAndName(x)).ToArray();
                return this.Ok(teams);
        }
        [HttpGet]
        public IHttpActionResult GetSingle(string id)
        {
            var team = this.Data.Teams.All().Where(x => x.NickName == id).FirstOrDefault();
            if (team == null)
            {
                return this.BadRequest("The requested team was not found in teams ");
            }
            var result = new TeamSingleViewModel(team);
            return this.Ok(result);
        }
    }
}
