using DomainModelling.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.DataAccessLayer
{
    public interface ICalendarRepository
    {
        Calendar Get(DateTime periodStart, DateTime periodEnd);

        void Save(Calendar calendar);
    }
}
