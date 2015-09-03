using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Infrastructure.Entities;

namespace TimeTracker.Infrastructure.Mappings
{
    public class TrackerHistoryMap : EntityTypeConfiguration<TrackerHistory>
    {
        public TrackerHistoryMap()
        {
            this.ToTable("TimeTrackerHistory", "dbo");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.ParentId).HasColumnName("ParentId");
            this.Property(t => t.DateCreated).HasColumnName("Date");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.MeetingMinutes).HasColumnName("MeetingMinutes");
            this.Property(t => t.ActiveMinutes).HasColumnName("ActiveMinutes");
            this.Property(t => t.DateModified).HasColumnName("LastUpdate");
            this.Property(t => t.SyncRoot).HasColumnName("SyncRoot").IsConcurrencyToken();

            // Relationships
            this.HasRequired(t => t.Tracker).WithMany(t => t.TrackerHistory).HasForeignKey(x => x.ParentId);
        }
    }
}
