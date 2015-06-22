using BetHelper.Data.Repositories;
using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Data
{
   public class BetHelperData:IBetHelperData
    {
       private DbContext context;
        private IDictionary<Type, object> repositories;

        public BetHelperData()
            : this(new BetHelperContext())
        {
        }        
        public BetHelperData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<ApplicationUser> Users
        {
            get
            {
                return this.GetRepository<ApplicationUser>();
            }
        }
        public IRepository<Game> Games
        {
            get { return this.GetRepository<Game>(); }
        }

        public IRepository<Player> Players
        {
            get { return this.GetRepository<Player>(); }
        }

        public IRepository<Division> Divisions
        {
            get { return this.GetRepository<Division>(); }
        }

        public IRepository<Team> Teams
        {
            get { return GetRepository<Team>(); }
        }

        public IRepository<Activity> Activities
        {
            get { return GetRepository<Activity>(); }
        }
        public IRepository<TeamGameProfile> TeamGameProfiles
        {
            get { return GetRepository<TeamGameProfile>(); }
        }
        public IRepository<RedCard> RedCards
        {
            get { return GetRepository<RedCard>(); }
        }
        public IRepository<RedCardProfileEvent> RedCardProfileEvents
        {
            get { return GetRepository<RedCardProfileEvent>(); }
        }
        public IRepository<Loan> Loans
        {
            get { return GetRepository<Loan>(); }
        }

        public IRepository<ManagerChange> ManagerChanges
        {
            get { return GetRepository<ManagerChange>(); }
        }
        public IRepository<DateFirstAppearance> DateFirstAppearances
        {
            get { return GetRepository<DateFirstAppearance>(); }
        }
        public IRepository<TeamFirstAppearance> TeamFirstAppearances
        {
            get { return GetRepository<TeamFirstAppearance>(); }
        }
       
        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }
    public Task<int> SaveChangesAsync()
        {
            return this.context.SaveChangesAsync();
        }
    public void Attach(Game game)
    {
         var context=(BetHelperContext)(this.context);
        context.Games.Attach(game);
    }
        private IRepository<T> GetRepository<T>() where T : class
        {
            var typeOfRepository = typeof(T);
            if (!this.repositories.ContainsKey(typeOfRepository))
            {
                var newRepository = Activator.CreateInstance(typeof(EFRepository<T>), context);
                this.repositories.Add(typeOfRepository, newRepository);
            }

            return (IRepository<T>)this.repositories[typeOfRepository];
        }
    }
}
