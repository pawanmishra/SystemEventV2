using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Core.Framework
{
    public abstract class TimeTrackerContextBase : DbContext
    {
        private readonly DateTime systemTime;

        static TimeTrackerContextBase()
        {
            Database.SetInitializer<TimeTrackerContextBase>(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackerContextBase" /> class
        /// with CommandTimeout of 120
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null</exception>
        protected TimeTrackerContextBase(string connectionString)
            : base(connectionString)
        {
            this.systemTime = DateTime.Now;

            // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.SavingChanges += ObjectContextOnSavingChanges;

            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = 120;
        }

        /// <summary>
        /// Inspired by
        /// http://dillieodigital.wordpress.com/2011/10/12/automatic-user-and-time-stamping-in-entity-framework-4/
        /// Explicit highlighting of modified properties in last if block, is required for detached entities. In case of
        /// detached entities, changes done in this method are not getting reflected back in the repository layer at
        /// the time of saving changes.
        /// </summary>
        private void ObjectContextOnSavingChanges(object sender, EventArgs eventArgs)
        {
            foreach (ObjectStateEntry entry in
                     ((ObjectContext)sender).ObjectStateManager
                       .GetObjectStateEntries(EntityState.Added | EntityState.Modified))
            {
                if (!entry.IsRelationship)
                {
                    var auditedEntity = entry.Entity as IEntity;
                    if (auditedEntity != null)
                    {
                        var touchtime = systemTime;
                        if (entry.State == EntityState.Added)
                            auditedEntity.DateCreated = touchtime;
                        auditedEntity.DateModified = touchtime;

                        auditedEntity.SyncRoot++;

                        if (entry.State == EntityState.Modified)
                        {
                            entry.SetModifiedProperty("SyncRoot");
                            entry.SetModifiedProperty("DateModified");
                        }
                    }
                }
            }            
        }
    }
}
