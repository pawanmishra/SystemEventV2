using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Constants;
using TimeTracker.Infrastructure.Dto;
using TimeTracker.Infrastructure.Entities;

namespace TimeTracker.Infrastructure.DtoMapper
{
    public class TrackerEntityToDtoMapper : IMapper<Tracker, TrackerDto>
    {
        public TrackerDto MapFrom(Tracker source, string rootUrl)
        {
            TrackerDto tracker = new TrackerDto();
            tracker.id = source.Id;
            tracker.user_name = source.UserName;
            tracker.date = source.DateCreated.ToString("dd-MM-yyyy");
            tracker.is_working_day = source.IsWorkingDay;
            tracker.active_minutes = source.ActiveMinutes;
            tracker.meeting_minutes = source.MeetingMinutes;
            tracker.start_time = source.StartTime.ToString("dd-MM-yyyy HH:mm:ss");
            tracker.last_update = source.DateModified.ToString("dd-MM-yyyy HH:mm:ss");
            tracker.tracker_history = string.Format("{0}{1}/{2}", rootUrl, UrlStrings.TrackerHistoryStem, tracker.id);

            return tracker;
        }
    }
}
