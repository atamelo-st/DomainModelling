using System;
using DomainModelling.DomainModel;

namespace DomainModelling.Application.Infrastructure
{
    public interface ICalendarRepository
    {
        Calendar Get(DateTime periodStart, DateTime periodEnd);

        int Save(Calendar calendar);
    }
}
