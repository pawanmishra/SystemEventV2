using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TimeTracker.Infrastructure;
using TimeTracker.Infrastructure.Dto;
using TimeTracker.Infrastructure.DtoMapper;
using TimeTracker.Infrastructure.Entities;
using TimeTracker.Infrastructure.Repository;

namespace TimeTracker.Web.Api.Controllers
{
    [EnableCors("http://localhost:62126", "*", "*")]
    [RouteAccept(TrackerApiStem, "application/json")]
    [RouteAccept(TrackerApiStem + "/{id}", "application/json")]
    public class TimeTrackerHistoryController : ApiController
    {
        private const string RootUrl = "http://localhost:50040/";
        private const string TrackerApiStem = "api" + "/" + "tracker_history";
        private readonly IRepository<TrackerHistory> _trackerRepository;
        private readonly IMapper<TrackerHistory, TrackerHistoryDto> _trackerMapper;

        public TimeTrackerHistoryController()
        {
            _trackerRepository = new EFRepository<TrackerHistory>();
            _trackerMapper = new TrackerHistoryEntityToDtoMapper();
        }

        public IHttpActionResult Get([FromUri] int id)
        {
            var trackerHistory = _trackerRepository.FindBy(x => x.ParentId == id).OrderBy(x => x.DateModified).ToList();
            if (trackerHistory != null)
            {
                var dto = trackerHistory.Select(x => _trackerMapper.MapFrom(x, RootUrl));
                return Ok(dto);
            }

            return NotFound();
        } 
    }
}
