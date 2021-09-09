using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace DomainModel
{
    public class RecurringEvent : Event
    {
        public static readonly DateTime? NoEndDate = null;

        public DateTime StartDate { get; }
        public DateTime? EndDate { get; }
        public RepeatPattern RepeatPattern { get; }

        private RecurringEvent(
            Guid id,
            string title,
            string description,
            DateTime startDate,
            DateTime? endDate,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            RepeatPattern repeatPattern) : base(id, title, description, startTime, endTime)
        {
            Guard.ThrowIf(startDate == default, nameof(startDate));
            Guard.ThrowIf(endDate <= startDate, nameof(startDate) + nameof(endDate));

            this.StartDate = startDate;
            this.EndDate = endDate;
            this.RepeatPattern = repeatPattern;
        }

        public IEnumerable<Occurrence> ExpandRecurrenceInterval(DateTime intervalStart, DateTime intervalEnd)
        {
            //NOTE: for ease of reading & debugging - eager evaluation
            //for big datasets it may be better to stay lazy

            IReadOnlyList<DateTime> recurrenceDates =
                this.RepeatPattern
                    .GetRecurrenceDatesForInterval(
                        intervalStart,
                        intervalEnd,
                        this.StartDate,
                        this.EndDate)
                    .ToList();

            IReadOnlyList<Occurrence> recurrentEventOccurrences =
                recurrenceDates
                    .Select(
                        recurrenceDate => new Occurrence(
                            this,
                            this.Title,
                            this.Description,
                            recurrenceDate,
                            this.StartTime,
                            this.EndTime))
                    .ToList();

            return recurrentEventOccurrences;
        }

        public static RecurringEvent Daily(
            Guid id,
            string title,
            string description,
            DateTime startDate,
            DateTime? endDate,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            int repeatInterval)
        {
            var repeatPattern = new RepeatPattern.Daily(repeatInterval);

            var dailyRecurringEvent = new RecurringEvent(
                id,
                title,
                description,
                startDate,
                endDate,
                startTime,
                endTime,
                repeatPattern);

            return dailyRecurringEvent;
        }

        public static RecurringEvent Weekly(
            Guid id,
            string title,
            string description,
            DateTime startDate,
            DateTime? endDate,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            RepeatPattern.Weekly.DaysOfWeek activeDays,
            int repeatInterval)
        {
            var repeatPattern = new RepeatPattern.Weekly(repeatInterval, activeDays);

            var weeklyRecurringEvent = new RecurringEvent(
                id,
                title,
                description,
                startDate,
                endDate,
                startTime,
                endTime,
                repeatPattern);

            return weeklyRecurringEvent;
        }

        //NOTE: this inheritance is just a shortcut; RecurringEvent.Occurence does NOT
        //seem to be in IS-A relationship with the RegularEvent
        public class Occurrence : RegularEvent
        {
            //NOTE We don't use this id for any operations
            //it's a smell and an indication of a shortcuts in design
            //a good design shouldn't have it at all in this case
            private static readonly Guid expandedOccurrenceStandInId = Guid.Empty;

            public RecurringEvent Parent { get; }
            public Occurrence(
                RecurringEvent parent,
                string title,
                string description,
                DateTime date,
                DateTimeOffset startTime,
                DateTimeOffset endTime) : base(expandedOccurrenceStandInId, title, description, date, startTime, endTime)
            {
                Guard.ThrowIf(parent is null, nameof(parent));

                Parent = parent;
            }
        }
    }
}