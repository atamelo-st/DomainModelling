using System;
using System.Collections.Generic;
using System.Linq;
using DomainModelling.DomainModel;

namespace DomainModelling.Common
{
    public static class CalendarExtensions
    {

        public static void AddRegularEvent(
            this Calendar calendar,
            Guid id,
            string title,
            string description,
            DateTime date,
            DateTimeOffset startTime,
            DateTimeOffset endTime)
        {
            var regularEvent = new RegularEvent(id, title, description, date, startTime, endTime);

            calendar.AddRegularEvent(regularEvent);
        }

        public static void UpdateRegularEvent(
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

            calendar.UpdateRegularEvent(updatedRegularEvent);
        }

        public static void UpdateRecurringEventOccurrence(
            this Calendar calendar,
            RecurringEvent parent,
            string newTitle,
            string newDescription,
            DateTime newDate,
            DateTimeOffset newStartTime,
            DateTimeOffset newEndTime)
        {
            var updatedOccurence =
                new RecurringEvent.Occurrence(
                    parent,
                    newTitle,
                    newDescription,
                    newDate,
                    newStartTime,
                    newEndTime
                );

            calendar.UpdateRecurringEventOccurrence(updatedOccurence);
        }
    }
}
