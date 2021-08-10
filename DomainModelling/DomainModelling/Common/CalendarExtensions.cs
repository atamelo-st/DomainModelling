using System;
using System.Collections.Generic;
using System.Linq;
using DomainModelling.DomainModel;

namespace DomainModelling.Common
{
    public static class CalendarExtensions
    {

        public static bool AddRegularEvent(
            this Calendar calendar,
            Guid id,
            string title,
            string description,
            DateTime date,
            DateTimeOffset startTime,
            DateTimeOffset endTime)
        {
            var regularEvent = new RegularEvent(id, title, description, date, startTime, endTime);

            return calendar.AddRegularEvent(regularEvent);
        }

        public static bool UpdateRegularEvent(
            this Calendar calendar,
            Guid id,
            string newTitle,
            string newDescription,
            DateTime newDate,
            DateTimeOffset newStartTime,
            DateTimeOffset newEndTime)
        {
            var updatedRegularEvent =
                new RegularEvent(
                    id,
                    newTitle,
                    newDescription,
                    newDate,
                    newStartTime,
                    newEndTime
                );

            return calendar.UpdateRegularEvent(updatedRegularEvent);
        }
    }
}
