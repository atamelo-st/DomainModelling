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

            IEnumerable<CalendarStorageItem> calendarData = this.GetCalendarData(periodStart, periodEnd);            

            foreach (CalendarStorageItem calendarItem in calendarData)
            {
                switch (calendarItem)
                {
                    case CalendarStorageItem.RegularEvent item:
                        {
                            // calendar.AddRegularEvent();
                            break;
                        }

                    case CalendarStorageItem.RecurringEvent item:
                        {
                            // calendar.AddRecurringEvent();
                            break;
                        }

                    case CalendarStorageItem.RecurringEventOverride item:
                        {
                            // calendar.UpdateRecurringEventOccurrence();
                            break;
                        }

                    case CalendarStorageItem.RecurringEventTombstone item:
                        {
                            // calendar.DeleteRecurringEventOccurrence();
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


        protected abstract IEnumerable<CalendarStorageItem> GetCalendarData(DateTime periodStart, DateTime periodEnd);

        protected abstract class CalendarStorageItem
        {
            public class RegularEvent : CalendarStorageItem { }

            public class RecurringEvent : CalendarStorageItem { }

            public class RecurringEventOverride : CalendarStorageItem { }

            public class RecurringEventTombstone : CalendarStorageItem { }
        }
    }
}
