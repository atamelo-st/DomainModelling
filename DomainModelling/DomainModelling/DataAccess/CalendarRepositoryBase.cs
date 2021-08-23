using DomainModelling.Application.Infrastructure;
using DomainModelling.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.DataAccess
{
    public abstract class CalendarRepositoryBase : ICalendarRepository
    {
        protected CalendarRepositoryBase()
        {
        }

        public Calendar Get(DateTime periodStart, DateTime periodEnd)
        {
            var calendar = new Calendar();

            IEnumerable<object> calendarData = this.GetCalendarData(periodStart, periodEnd);            

            foreach (Event calendarItem in calendarData)
            {
                switch (calendarItem)
                {
                    //TODO: figure out what to return for an occurence override
                    case RecurringEvent.Occurrence evt:
                        {
                            // calendar.UpdateRecurringEventOccurrence();
                            break;
                        }

                    // TODO: handle tombstone case

                    case RegularEvent evt:
                        {
                            calendar.AddRegularEvent(evt);
                            break;
                        }

                    case RecurringEvent evt:
                        {
                            calendar.AddRecurringEvent(evt);
                            break;
                        }

                    default: throw new ArgumentException();
                }
            }

            //NOTE: 'resetting' the events published due to the state rebuild 
            calendar.AcknowledgeDomainEvents();

            return calendar;
        }


        public abstract void Save(Calendar calendar);


        // NOTE: returns regular events, recurring events, as well as occurrences overrides & tombstones
        protected abstract IEnumerable<object> GetCalendarData(DateTime periodStart, DateTime periodEnd);
    }
}
