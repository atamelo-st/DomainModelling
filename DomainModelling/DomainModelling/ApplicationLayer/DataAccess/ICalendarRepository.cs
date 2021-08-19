using System;
using DomainModelling.DomainModel;

namespace DomainModelling.ApplicationLayer.DataAccess
{
    public interface ICalendarRepository
    {
        Calendar Get(DateTime periodStart, DateTime periodEnd);

        int Save(Calendar calendar);
    }
}
