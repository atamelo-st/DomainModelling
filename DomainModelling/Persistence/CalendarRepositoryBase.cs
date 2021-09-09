using System;
using DomainModel;
using DomainModelling.Application.Infrastructure;

namespace Persistence
{
    public abstract class CalendarRepositoryBase : ICalendarRepository
    {
        protected CalendarRepositoryBase()
        {
        }

        public Calendar Get(DateTime periodStart, DateTime periodEnd)
        {
            Calendar calendar = this.RehydrateInstance(periodStart, periodEnd);

            //NOTE: 'resetting' the events published due to the state rebuild 
            calendar.AcknowledgeDomainEvents();

            return calendar;
        }

        protected abstract Calendar RehydrateInstance(DateTime periodStart, DateTime periodEnd);

        public abstract void Save(Calendar calendar);
    }
}
