using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Framework;

namespace TimeTracker.Infrastructure.Entities
{
    public class TrackerHistory : IEntity
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string UserName { get; set; }

        public int MeetingMinutes { get; set; }

        public int ActiveMinutes { get; set; }

        public int SyncRoot { get; set; }

        public Tracker Tracker { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
