using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Migration.Core
{
    public class TrackerMigrationAttribute : MigrationAttribute
    {
        public TrackerMigrationAttribute(uint year, uint month, uint day, uint hour, uint minute)
            : base(CreateVersionFromDateParts(year, month, day, hour, minute))
        {
        }

        private static long CreateVersionFromDateParts(uint year, uint month, uint day, uint hour, uint minute)
        {
            var stringifiedVersion = string.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}", year, month, day, hour, minute);
            var longifiedVersion = long.Parse(stringifiedVersion);

            return longifiedVersion;
        }
    }
}
