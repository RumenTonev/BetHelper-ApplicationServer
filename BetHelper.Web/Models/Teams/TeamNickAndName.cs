using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BetHelper.Web.Models.Teams
{
    public class TeamNickAndName
    {
         public TeamNickAndName(Team team)
        {
            
            this.Name = team.Name;
             this.Nick=team.NickName;

           
            
           
        }
        public string Name { get; set; }
        public string Nick { get; set; }
       
    }
}