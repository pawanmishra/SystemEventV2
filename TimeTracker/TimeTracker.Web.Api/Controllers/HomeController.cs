using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimeTracker.Infrastructure;

namespace TimeTracker.Web.Api.Controllers
{
    [RouteAccept("", "application/json")]
    public class HomeController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok("Hello World!");
        }
    }
}
