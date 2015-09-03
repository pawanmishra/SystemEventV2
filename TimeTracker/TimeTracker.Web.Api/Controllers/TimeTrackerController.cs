using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
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
    public class TimeTrackerController : ApiController
    {
        private const string RootUrl = "http://localhost:50040/";
        private const string TrackerApiStem = "api" + "/" + "tracker";
        private readonly IRepository<Tracker> _trackerRepository;
        private readonly IMapper<Tracker, TrackerDto> _trackerMapper;

        public TimeTrackerController()
        {
            _trackerRepository = new EFRepository<Tracker>();
            _trackerMapper = new TrackerEntityToDtoMapper();
        }

        public IEnumerable<TrackerDto> Get()
        {
            return _trackerRepository.GetAll().ToList()
                .OrderBy(x => x.StartTime)
                .Select(x => _trackerMapper.MapFrom(x, RootUrl));
        }
        
        public IHttpActionResult Get([FromUri] int id)
        {
            var tracker = _trackerRepository.FindBy(x => x.Id == id).FirstOrDefault();
            if(tracker != null)
            {
                var dto = _trackerMapper.MapFrom(tracker, RootUrl);
                return Ok(dto);
            }

            return NotFound();
        }

        public IHttpActionResult Get(string startDate, string endDate)
        {
            try
            {
                DateTime start = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var result =
                    _trackerRepository.FindBy(x => x.StartTime >= start && x.StartTime <= end)
                        .ToList()
                        .OrderBy(z => z.StartTime)
                        .Select(t => _trackerMapper.MapFrom(t, RootUrl));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}