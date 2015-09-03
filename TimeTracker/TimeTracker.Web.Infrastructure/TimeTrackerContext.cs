using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Framework;
using TimeTracker.Infrastructure.Entities;
using TimeTracker.Infrastructure.Mappings;

namespace TimeTracker.Infrastructure
{
    public class TimeTrackerContext : TimeTrackerContextBase
    {
        static TimeTrackerContext()
        {
            Database.SetInitializer<TimeTrackerContext>(null);
        }

        public TimeTrackerContext(string connectionString)
            : base(connectionString)
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = 120;
        }

        public DbSet<Tracker> Tracker { get; set; }

        public DbSet<TrackerHistory> TrackerHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Configuration.LazyLoadingEnabled = false;
            //// ToDo Is this setting required?

            //// Configuration.AutoDetectChangesEnabled = false;

            modelBuilder.Configurations.Add(new TrackerMap());
            modelBuilder.Configurations.Add(new TrackerHistoryMap());
        }
    }
}
