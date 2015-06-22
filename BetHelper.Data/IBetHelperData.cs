using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetHelper.Data.Repositories;

namespace BetHelper.Data
{
    public interface IBetHelperData
    {
        IRepository<ApplicationUser> Users { get; }
        IRepository<Game> Games { get; }

        IRepository<Division> Divisions { get; }

        IRepository<Player> Players { get; }

        IRepository<Team> Teams { get; }
        IRepository<Activity> Activities { get; }
        IRepository<TeamGameProfile> TeamGameProfiles { get; }
        IRepository<RedCard> RedCards { get; }
        IRepository<RedCardProfileEvent> RedCardProfileEvents { get; }
        IRepository<Loan> Loans { get; }
        IRepository<ManagerChange> ManagerChanges { get; }
        IRepository<DateFirstAppearance> DateFirstAppearances { get; }

        IRepository<TeamFirstAppearance> TeamFirstAppearances { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
