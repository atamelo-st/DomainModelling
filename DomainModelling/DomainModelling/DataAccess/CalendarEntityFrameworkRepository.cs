using DomainModelling.Application.Infrastructure;
using DomainModelling.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.DataAccess
{
    public class CalendarEntityFrameworkRepository : CalendarRepositoryBase
    {
        public override void Save(Calendar calendar)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object> GetCalendarData(DateTime periodStart, DateTime periodEnd)
        {
            throw new NotImplementedException();
        }
    }
}
