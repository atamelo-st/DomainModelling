using DomainModelling.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.DomainModel.DomainEvents
{
    public abstract record DomainEvent
    {
        public Guid Id { get; }

        private DomainEvent()
        {
            this.Id = Guid.NewGuid();
        }
        
        public record RegularEventAdded(RegularEvent AddedEvent) : DomainEvent;

        public record RegularEventUpdated(RegularEvent OriginalEvent, RegularEvent UpdatedEvent) : DomainEvent;
        
        public record RegularEventDeleted(RegularEvent DeletedEvent) : DomainEvent;

        public record RecurringEventAdded(RecurringEvent AddedEvent) : DomainEvent;

        public record RecurringEventUpdated(RecurringEvent OriginalEvent, RecurringEvent UpdatedEvent) : DomainEvent;
        
        public record RecurringEventOccurrenceUpdated
            (RecurringEvent.Occurrence OriginalEventOccurrence, RecurringEvent.Occurrence UpdatedEventOccurrence)
                : DomainEvent;

        public record RecurringEventOccurrenceDeleted(RecurringEvent ParentRecurringEvent, DateTime Date) : DomainEvent;
    }
}
