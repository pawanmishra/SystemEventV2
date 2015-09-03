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
    public class TrackerHistoryEntityToDtoMapper : IMapper<TrackerHistory, TrackerHistoryDto>
    {
        public TrackerHistoryDto MapFrom(TrackerHistory item, string rootUrl)
        {
            TrackerHistoryDto dto = new TrackerHistoryDto();
            dto.id = item.Id;
            dto.parent_id = item.ParentId;
            dto.user_name = item.UserName;
            dto.date = item.DateCreated.ToString("dd-MM-yyyy");
            dto.active_minutes = item.ActiveMinutes;
            dto.meeting_minutes = item.MeetingMinutes;
            dto.last_update = item.DateModified.ToString("HH:mm:ss");
            dto.parent_tracker = string.Format("{0}{1}/{2}", rootUrl, UrlStrings.TrackerStem, dto.parent_id);
            return dto;
        }
    }
}
