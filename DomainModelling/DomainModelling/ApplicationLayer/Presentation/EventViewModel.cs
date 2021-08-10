using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.ApplicationLayer.Presentation
{
    public class EventViewModel
    {
        public Guid Id { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime Date { get; }
        public DateTimeOffset StartTime { get; }
        public DateTimeOffset EndTime { get; }
        public RecurringEventData RecurringEventData { get; }
        public bool IsRecurringEvent { get => this.RecurringEventData != null; }

        public EventViewModel(
            Guid id,
            string title,
            string description,
            DateTime date,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            RecurringEventData recurringEventData)
        {
            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.Date = date;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.RecurringEventData = recurringEventData;
        }
    }

    public class RecurringEventData
    {
        public Guid ParentRecurringEventId { get; }

        // TODO: 'object' is just for simplicity; need to flesh the real pattern representation here
        public object RepeatPattern { get; }

        public RecurringEventData(Guid parentRecurringEventId, object repeatPattern)
        {
            ParentRecurringEventId = parentRecurringEventId;
            RepeatPattern = repeatPattern;
        }
    }
}
