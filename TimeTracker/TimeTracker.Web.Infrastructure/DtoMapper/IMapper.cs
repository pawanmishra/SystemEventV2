using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Infrastructure.DtoMapper
{
    public interface IMapper<in TSource, out TDestination>
    {
        TDestination MapFrom(TSource source, string url);
    }
}
