using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetHelper.Web.Models.Profiles
{
    public class ProfilesUpdateModel
    {
        
        public int ProfileId { get; set; }
        public string CurrentFormLevelNumber { get; set; }
    }
}