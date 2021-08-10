using System;
using DomainModelling.Common;


namespace DomainModelling.DomainModel
{
    public class RegularEvent : Event
    {
        public DateTime Date { get; }

        public RegularEvent(
            Guid id,
            string title,
            string description,
            DateTime date,
            DateTimeOffset startTime,
            DateTimeOffset endTime) : base(id, title, description, startTime, endTime)
        {
            Guard.ThrowIf(date == default, nameof(date));

            this.Date = date;
        }
    }
}