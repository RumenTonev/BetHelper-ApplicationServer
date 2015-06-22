//using BetHelper.Data.Migrations;
using BetHelper.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetHelper.Data
{
    public class BetHelperContext : IdentityDbContext<ApplicationUser>
    {

        public BetHelperContext()
            : base("BetHelperDb", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BetHelperContext, Configuration>());
        }

        public BetHelperContext(string connString)
            : base(connString)
        {
        }


        public IDbSet<TeamGameProfile> TeamGameProfiles { get; set; }
        public IDbSet<Activity> Activities { get; set; }
        public IDbSet<Loan> Loans { get; set; }
        public IDbSet<Game> Games { get; set; }

        public IDbSet<Player> Players { get; set; }

        public IDbSet<Division> Divisons { get; set; }

        public IDbSet<Team> Teams { get; set; }
        public IDbSet<RedCard> RedCards { get; set; }
        public IDbSet<RedCardProfileEvent> RedCardProfileEvents { get; set; }


        public IDbSet<TeamFirstAppearance> TeamFirstAppearances { get; set; }
        public IDbSet<DateFirstAppearance> DateFirstAppearances { get; set; }

        public IDbSet<ManagerChange> ManagerChanges { get; set; }
        public static BetHelperContext Create()
        {
            return new BetHelperContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure Code First to ignore PluralizingTableName convention 
            // If you keep this convention then the generated tables will have pluralized names. 
            // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RedCard>()
                .HasKey(t => t.ActivityId);

            // Map one-to-zero or one relationship 
            modelBuilder.Entity<RedCard>()
                .HasRequired(t => t.Activity)
                .WithOptional(t => t.RedCard);
        }
    }
}
