using DomainModelling.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.DomainModel.DomainEvents
{
    public abstract class DomainEvent
    {
        public Guid Id { get; }


        private DomainEvent(Guid id)
        {
            Guard.ThrowIf(id == default, nameof(id));

            Id = id;
        }

        public class RegularEventAdded : DomainEvent
        {
            public RegularEvent AddedEvent { get; }

            public RegularEventAdded(RegularEvent addedEvent) : this(Guid.NewGuid(), addedEvent)
            {
            }

            public RegularEventAdded(Guid id, RegularEvent addedEvent) : base(id)
            {
                Guard.ThrowIf(addedEvent == null, nameof(addedEvent));

                this.AddedEvent = addedEvent;
            }
        }

        public class RegularEventUpdated : DomainEvent
        {
            public RegularEvent OriginalEvent { get; }
            public RegularEvent UpdatedEvent { get; }

            public RegularEventUpdated(RegularEvent originalEvent, RegularEvent updatedEvent)
                : this(Guid.NewGuid(), originalEvent, updatedEvent)
            {
            }

            public RegularEventUpdated(Guid id, RegularEvent originalEvent, RegularEvent updatedEvent) : base(id)
            {
                Guard.ThrowIf(originalEvent == null, nameof(originalEvent));
                Guard.ThrowIf(updatedEvent == null, nameof(updatedEvent));

                OriginalEvent = originalEvent;
                UpdatedEvent = updatedEvent;
            }
        }

        public class RegularEventDeleted : DomainEvent
        {
            public RegularEvent DeletedEvent { get; }

            public RegularEventDeleted(RegularEvent deletedEvent) : this(Guid.NewGuid(), deletedEvent)
            {
            }

            public RegularEventDeleted(Guid id, RegularEvent deletedEvent) : base(id)
            {
                Guard.ThrowIf(deletedEvent == null, nameof(deletedEvent));

                DeletedEvent = deletedEvent;
            }
        }

        public class RecurringEventAdded : DomainEvent
        {
            public RecurringEvent AddedEvent { get; }

            public RecurringEventAdded(RecurringEvent addedEvent) : this(Guid.NewGuid(), addedEvent)
            {
            }

            public RecurringEventAdded(Guid id, RecurringEvent addedEvent) : base(id)
            {
                Guard.ThrowIf(addedEvent == null, nameof(addedEvent));

                AddedEvent = addedEvent;
            }
        }

        public class RecurringEventUpdated : DomainEvent
        {
            public RecurringEvent OriginalEvent { get; }
            public RecurringEvent UpdatedEvent { get; }

            public RecurringEventUpdated(RecurringEvent originalEvent, RecurringEvent updatedEvent)
                : this(Guid.NewGuid(), originalEvent, updatedEvent)
            {
            }

            public RecurringEventUpdated(Guid id, RecurringEvent originalEvent, RecurringEvent updatedEvent) : base(id)
            {
                Guard.ThrowIf(originalEvent == null, nameof(originalEvent));
                Guard.ThrowIf(updatedEvent == null, nameof(updatedEvent));

                OriginalEvent = originalEvent;
                UpdatedEvent = updatedEvent;
            }
        }

        public class RecurringEventOccurrenceUpdated : DomainEvent
        {
            public RecurringEvent.Occurrence OriginalEventOccurrence { get; }
            public RecurringEvent.Occurrence UpdatedEventOccurrence { get; }

            public RecurringEventOccurrenceUpdated(RecurringEvent.Occurrence originalEventOccurrence, RecurringEvent.Occurrence updatedEventOccurrence) 
                : this(Guid.NewGuid(), originalEventOccurrence, updatedEventOccurrence)
            {
            }

            public RecurringEventOccurrenceUpdated(Guid id, RecurringEvent.Occurrence originalEventOccurrence, RecurringEvent.Occurrence updatedEventOccurrence) 
                : base(id)
            {
                Guard.ThrowIf(originalEventOccurrence == null, nameof(originalEventOccurrence));
                Guard.ThrowIf(updatedEventOccurrence == null, nameof(updatedEventOccurrence));

                OriginalEventOccurrence = originalEventOccurrence;
                UpdatedEventOccurrence = updatedEventOccurrence;
            }
        }

        public class RecurringEventOccurrenceDeleted : DomainEvent
        {

            public RecurringEvent ParentRecurringEvent { get; }
            public DateTime Date { get; }

            public RecurringEventOccurrenceDeleted(RecurringEvent parentRecurringEvent, DateTime date)
                : this(Guid.NewGuid(), parentRecurringEvent, date)
            {
            }

            public RecurringEventOccurrenceDeleted(Guid id, RecurringEvent parentRecurringEvent, DateTime date) : base(id)
            {
                Guard.ThrowIf(parentRecurringEvent == null, nameof(parentRecurringEvent));
                Guard.ThrowIf(date == default, nameof(date));

                ParentRecurringEvent = parentRecurringEvent;
                Date = date;
            }
        }
    }
}
