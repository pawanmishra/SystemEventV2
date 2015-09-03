using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Infrastructure.Dto
{
    public class TrackerDto
    {
        public int id { get; set; }
        public string date { get; set; }
        public string user_name { get; set; }
        public int is_working_day { get; set; }
        public int meeting_minutes { get; set; }
        public int active_minutes { get; set; }
        public string start_time { get; set; }
        public string last_update { get; set; }
        public string tracker_history { get; set; }
    }
}
