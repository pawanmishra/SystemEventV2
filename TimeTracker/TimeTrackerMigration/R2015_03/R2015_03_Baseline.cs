using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Migration.Core;

namespace TimeTrackerMigration.R2015_03
{
    [TrackerMigration(2015, 03, 03, 0, 0)]
    public class R2015_03_Baseline : ForwardOnlyMigration
    {
        private static string kDbo = "dbo";

        public override void Up()
        {
            if (!Schema.Schema(kDbo).Exists())
                Create.Schema(kDbo);

            Create.Table("TimeTracker")
                .InSchema(kDbo)
                .WithColumn("Id").AsInt32().NotNullable()
                .WithColumn("UserName").AsString().NotNullable()
                .WithColumn("Date").AsDateTime().NotNullable()
                .WithColumn("MeetingMinutes").AsInt32().Nullable()
                .WithColumn("ActiveMinutes").AsInt32().Nullable()
                .WithColumn("IsWorkingDay").AsBoolean().Nullable()
                .WithColumn("StartTime").AsDateTime().NotNullable()
                .WithColumn("LastUpdate").AsDateTime().NotNullable()
                .WithColumn("SyncRoot").AsInt32().NotNullable().WithDefaultValue(1);
            Create.PrimaryKey("PK_TimeTracker").OnTable("TimeTracker").WithSchema(kDbo).Columns(new[] { "Id" });

            Create.Table("TimeTrackerHistory")
                .InSchema(kDbo)
                .WithColumn("Id").AsInt32().NotNullable()
                .WithColumn("ParentId").AsInt32().NotNullable()
                .WithColumn("Date").AsDateTime().NotNullable()
                .WithColumn("MeetingMinutes").AsInt32().Nullable()
                .WithColumn("ActiveMinutes").AsInt32().Nullable()
                .WithColumn("UserName").AsString().NotNullable()
                .WithColumn("LastUpdate").AsDateTime().NotNullable()
                .WithColumn("SyncRoot").AsInt32().NotNullable().WithDefaultValue(1);
            Create.PrimaryKey("PK_TimeTracker_History").OnTable("TimeTrackerHistory").WithSchema(kDbo).Columns(new[] { "Id" });

            Create.ForeignKey("FK_TimeTrackerHistory_TimeTracker")
                .FromTable("TimeTrackerHistory")
                .InSchema(kDbo)
                .ForeignColumns(new[] { "ParentId" })
                .ToTable("TimeTracker")
                .InSchema(kDbo)
                .PrimaryColumns(new[] { "Id" });
        }
    }
}
