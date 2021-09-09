using System;
using System.Collections.Generic;

namespace DomainModel
{
    public abstract class RepeatPattern
    {
        public int RepeatInterval { get; }

        private RepeatPattern(int repeatInterval)
        {
            this.RepeatInterval = repeatInterval;
        }

        // Gets all the dates for the intersection of the requested interval and recurrence
        public abstract IEnumerable<DateTime> GetRecurrenceDatesForInterval(
            DateTime intervalStart,
            DateTime intervalEnd,
            DateTime recurrenceStart,
            DateTime? recurrenceEnd);

        private DateTime CalculateEndDate(DateTime intervalEnd, DateTime? recurrenceEnd)
        {
            DateTime lastDay =
                recurrenceEnd == RecurringEvent.NoEndDate ?
                    intervalEnd :
                    intervalEnd < recurrenceEnd.Value ? intervalEnd : recurrenceEnd.Value;

            return lastDay;
        }

        public class Daily : RepeatPattern
        {
            public Daily(int repeatInterval) : base(repeatInterval)
            { }

            public override IEnumerable<DateTime> GetRecurrenceDatesForInterval(
                DateTime intervalStart,
                DateTime intervalEnd,
                DateTime recurrenceStart,
                DateTime? recurrenceEnd)
            {
                DateTime lastDay = this.CalculateEndDate(intervalEnd, recurrenceEnd);

                DateTime currentDay = recurrenceStart;

                while (currentDay <= lastDay)
                {
                    if (currentDay >= intervalStart)
                    {
                        yield return currentDay;
                    }

                    currentDay = currentDay.AddDays(this.RepeatInterval);
                }
            }
        }

        public class Weekly : RepeatPattern
        {
            public DaysOfWeek ActiveDays { get; }

            public Weekly(int repeatInterval, DaysOfWeek activeDays)
                : base(repeatInterval)
            {
                ActiveDays = activeDays;
            }

            public override IEnumerable<DateTime> GetRecurrenceDatesForInterval(
                DateTime intervalStart,
                DateTime intervalEnd,
                DateTime recurrenceStart,
                DateTime? recurrenceEnd)
            {
                DateTime lastWeek = this.CalculateEndDate(intervalEnd, recurrenceEnd);

                DateTime currentWeek = recurrenceStart;

                while (currentWeek <= lastWeek)
                {
                    DateTime currentWeekDay = currentWeek;
                    DateTime lastWeekDay = currentWeek.AddDays(7);

                    while (currentWeekDay <= lastWeekDay)
                    {
                        if (this.IsActiveOnDayOfWeek(currentWeekDay.DayOfWeek) && currentWeekDay >= intervalStart)
                        {
                            yield return currentWeekDay;
                        }

                        currentWeekDay = currentWeekDay.AddDays(1);
                    }

                    currentWeek = currentWeek.AddDays(7 * this.RepeatInterval);
                }
            }

            private bool IsActiveOnDayOfWeek(DayOfWeek dayOfWeek)
            {
                int mask = 1 << (int)dayOfWeek;

                bool isActive = ((int)this.ActiveDays & mask) == mask;

                return isActive;
            }

            [Flags]
            public enum DaysOfWeek
            {
                None = 0,
                Sunday = 1,
                Monday = 2,
                Tuesday = 4,
                Wednesday = 8,
                Thursday = 16,
                Friday = 32,
                Saturday = 64
            }
        }
    }
}