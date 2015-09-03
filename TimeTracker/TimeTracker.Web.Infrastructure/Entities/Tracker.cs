using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Framework;

namespace TimeTracker.Infrastructure.Entities
{
    public class Tracker : IEntity
    {
        public Tracker()
        {
            TrackerHistory = new List<TrackerHistory>();
        }

        public string UserName { get; set; }

        public int MeetingMinutes { get; set; }

        public int ActiveMinutes { get; set; }

        public int IsWorkingDay { get; set; }

        public DateTime StartTime { get; set; }

        public IList<TrackerHistory> TrackerHistory { get; set; }

        public int Id { get; set; }

        public int SyncRoot { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
