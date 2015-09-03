using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http.Routing;

namespace TimeTracker.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RouteAcceptAttribute : RouteFactoryAttribute, IHttpRouteConstraint
    {
        private readonly string _acceptableType;

        public RouteAcceptAttribute(string template, string acceptableType)
            : base(template)
        {
            _acceptableType = acceptableType;
        }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary { { "version", this } };
                return constraints;
            }
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection != HttpRouteDirection.UriResolution)
            {
                return false;
            }
            if (_acceptableType == null)
            {
                return request.Headers.Accept.Count == 0;
            }
            return request.Headers.Accept.Any(a => a.MediaType == _acceptableType);
        }
    }
}
