using System;
using System.Collections.Generic;
using System.Linq;
using DomainModelling.DomainModel.DomainEvents;


namespace DomainModelling.DomainModel
{
    public class Calendar
    {
        //NOTE: just for a quick and dirty implementation; interval trees are needed for a prod-ready code
        private readonly HashSet<RegularEvent> _regularEvents; //TODO: move to Dictionary
        private readonly IDictionary<Guid, RecurringEvent> _recurringEvents;
        //NOTE: RecurringEvent(aka parent) -> occurence overrides
        private readonly IDictionary<Guid, IDictionary<DateTime, RecurringEvent.Occurrence>> _recurringOccurrencesOverrides;
        private readonly IDictionary<Guid, HashSet<DateTime>> _recurringOccurrencesTombstones;
        private readonly List<DomainEvent> _domainEvents;

        public IEnumerable<DomainEvent> DomainEvents => this._domainEvents;


        public Calendar()
        {
            this._regularEvents = new HashSet<RegularEvent>();
            this._recurringEvents = new Dictionary<Guid, RecurringEvent>();
            this._recurringOccurrencesOverrides = new Dictionary<Guid, IDictionary<DateTime, RecurringEvent.Occurrence>>();
            this._recurringOccurrencesTombstones = new Dictionary<Guid, HashSet<DateTime>>();
            this._domainEvents = new List<DomainEvent>();
        }


        public IEnumerable<Event> GetAllEvents()
        {
            IEnumerable<Event> events = this.GetEvents(DateTime.MinValue, DateTime.MaxValue);

            return events;
        }


        public bool AddRegularEvent(RegularEvent regularEvent)
        {
            bool added = this._regularEvents.Add(regularEvent);

            if (added)
            {
                var regularEventAdded = new DomainEvent.RegularEventAdded(regularEvent);

                this.PublishDomainEvent(regularEventAdded);
            }

            return added;
        }


        public bool UpdateRegularEvent(RegularEvent regularEvent)
        {
            //NOTE: since we've provided custom equality logic, only event.Id is taken into account when event is searched/deleted.
            //So to update an already existing event, just pass in a new instance with the same Id and updated fields
            if (!this._regularEvents.Remove(regularEvent))
            {
                return false;
            }

            bool updated = this._regularEvents.Add(regularEvent);

            if (updated)
            {
                RegularEvent originalEvent = null; //TODO: extract original event
                RegularEvent updatedEvent = regularEvent;
                var regularEventUpdated = new DomainEvent.RegularEventUpdated(originalEvent, updatedEvent);
                
                this.PublishDomainEvent(regularEventUpdated);
            }

            return updated;
        }


        public bool DeleteRegularEvent(RegularEvent regularEvent)
        {
            bool deleted = this._regularEvents.Remove(regularEvent);

            if (deleted)
            {
                var regularEventDeleted = new DomainEvent.RegularEventDeleted(regularEvent);

                this.PublishDomainEvent(regularEventDeleted);
            }

            return deleted;
        }


        public bool AddRecurringEvent(RecurringEvent recurringEvent)
        {
            if (this.RecurringEventExists(recurringEvent.Id))
            {
                return false;
            }

            this._recurringEvents.Add(recurringEvent.Id, recurringEvent);

            var recurringEventAdded = new DomainEvent.RecurringEventAdded(recurringEvent);
            this.PublishDomainEvent(recurringEventAdded);

            return true;
        }


        public bool UpdateRecurringEvent(RecurringEvent recurringEvent)
        {
            if (!this._recurringEvents.TryGetValue(recurringEvent.Id, out RecurringEvent originalRecurringEvent))
            {
                return false;
            }

            this._recurringEvents.Remove(recurringEvent.Id);
            this._recurringEvents.Add(recurringEvent.Id, recurringEvent);

            RecurringEvent updatedRecurringEvent = recurringEvent;
            var recurringEventUpdated = new DomainEvent.RecurringEventUpdated(originalRecurringEvent, updatedRecurringEvent);
            this.PublishDomainEvent(recurringEventUpdated);

            return true;
        }
        

        public bool UpdateRecurringEventOccurrence(
            Guid parentRecurringEventId,
            DateTime date,
            string newTitle,
            string newDescription,
            DateTimeOffset newStartTime,
            DateTimeOffset newEndTime)
        {
            if (!this.RecurringEventExists(parentRecurringEventId))
            {
                return false;
            }

            //NOTE: I've chosen this semantics as the simplest - can't update a deleted occurence.
            //As an alternative, we could consider possibility of resurrecting an occurence -
            //e.g. if it was deleted by mistake, updating a deleted item would 'bring it back'..
            if (this.IsOccurrenceDeleted(parentRecurringEventId, date))
            {
                return false;
            }
            
            if (!this.RecurringEventOccurrenceExists(parentRecurringEventId, date))
            {
                return false;
            }

            RecurringEvent.Occurrence originalOccurence = null;

            //Recurrent event has overrides?
            if (this._recurringOccurrencesOverrides.TryGetValue(parentRecurringEventId, out var occurrenceOverrides))
            {
                //If there is an old override - remove it
                if (occurrenceOverrides.TryGetValue(date, out originalOccurence))
                {
                    occurrenceOverrides.Remove(date);
                }
            }
            else //No overrides - create it
            {
                occurrenceOverrides = new Dictionary<DateTime, RecurringEvent.Occurrence>();
                this._recurringOccurrencesOverrides.Add(parentRecurringEventId, occurrenceOverrides);
            }

            RecurringEvent parent = this._recurringEvents[parentRecurringEventId];

            var updatedOccurence =
                new RecurringEvent.Occurrence(
                    parent,
                    newTitle,
                    newDescription,
                    date,
                    newStartTime,
                    newEndTime);

            //add submitted override
            occurrenceOverrides.Add(date, updatedOccurence);

            var recurringEventOccurrenceUpdated = new DomainEvent.RecurringEventOccurrenceUpdated(originalOccurence, updatedOccurence);
            this.PublishDomainEvent(recurringEventOccurrenceUpdated);

            return true;
        }


        public bool DeleteRecurringEventOccurrence(RecurringEvent.Occurrence occurence)
        {
            bool deleted = this.DeleteRecurringEventOccurrence(occurence.Parent.Id, occurence.Date);

            return deleted;
        }

 
        public bool DeleteRecurringEventOccurrence(Guid parentRecurringEventId, DateTime date)
        {
            if (!this.RecurringEventExists(parentRecurringEventId))
            {
                return false;
            }

            if (!this._recurringOccurrencesTombstones.TryGetValue(parentRecurringEventId, out var tombstones))
            {
                tombstones = new HashSet<DateTime>();

                this._recurringOccurrencesTombstones.Add(parentRecurringEventId, tombstones);
            }

            tombstones.Add(date);

            this._recurringEvents.TryGetValue(parentRecurringEventId, out RecurringEvent parentRecurringEvent);

            var recurringEventOccurrenceDeleted = new DomainEvent.RecurringEventOccurrenceDeleted(parentRecurringEvent, date);
            this.PublishDomainEvent(recurringEventOccurrenceDeleted);

            // NOTE: idempotent delete
            return true;
        }
        

        //NOTE: this code craves for discriminated unions :)
        //NOTE: need interval trees data structure here; right now - naive brute-force
        //NOTE: the output is no particular order
        public IEnumerable<Event> GetEvents(DateTime startDate, DateTime endDate)
        {
            foreach (RegularEvent regularEvent in this._regularEvents)
            {
                if (regularEvent.Date >= startDate && regularEvent.Date <= endDate)
                {
                    yield return regularEvent;
                }
            }

            foreach (RecurringEvent recurringEvent in this._recurringEvents.Values)
            {
                IEnumerable<RecurringEvent.Occurrence> recurrentOccurrences =
                    recurringEvent
                        .ExpandRecurrenceInterval(startDate, endDate)
                        //Skip deleted
                        .Where(occurrence => !this.IsOccurrenceDeleted(occurrence))
                        //Apply override if exists
                        .Select(this.ResolveOccurenceOverride);

                foreach (RecurringEvent.Occurrence recurrentOccurrence in recurrentOccurrences)
                {
                    yield return recurrentOccurrence;
                }
            }
        }
        

        public static bool DatesWithinAllowedRange(DateTime periodStart, DateTime periodEnd)
        {
            bool withinRange = Math.Abs((periodEnd - periodStart).Days) <= 7;

            return withinRange;
        }
        
        
        public bool RecurringEventOccurrenceExists(Guid parentRecurringEventId, DateTime date)
        {
            //TODO: implement the logic to check if an occurrence for the parent event
            //exists for a given date.
            //1. Extract recurring event
            //2. Delegate the check to the event object
            
            return true;
        }


        public void AcknowledgeDomainEvents()
        {
            this._domainEvents.Clear();
        }
        

        private RecurringEvent.Occurrence ResolveOccurenceOverride(RecurringEvent.Occurrence recurringOccurrence)
        {
            RecurringEvent.Occurrence @override =
                this.FindOccurrenceOverride(recurringOccurrence.Parent.Id, recurringOccurrence.Date);

            RecurringEvent.Occurrence resolvedOverride = @override ?? recurringOccurrence;

            return resolvedOverride;
        }


        private RecurringEvent.Occurrence FindOccurrenceOverride(Guid recurringOccurenceParentId, DateTime date)
        {
            //Recurrent event has overrides?
            if (this._recurringOccurrencesOverrides.TryGetValue(recurringOccurenceParentId, out var occurrenceOverrides))
            {
                //NOTE: the compound primary key for an override occurence
                // is (Parent, Date) - i.e. for a given recurring event there could be only one
                // occurence with a unique combination of those
                occurrenceOverrides.TryGetValue(date, out RecurringEvent.Occurrence found);

                return found;
            }

            return null;
        }


        private bool IsOccurrenceDeleted(RecurringEvent.Occurrence occurrence)
        { 
            return this.IsOccurrenceDeleted(occurrence.Parent.Id, occurrence.Date);
        }


        private bool IsOccurrenceDeleted(Guid parentRecurringEventId, DateTime date)
        {
            if (this._recurringOccurrencesTombstones.TryGetValue(parentRecurringEventId, out var dates))
            {
                bool isDeleted = dates.Contains(date);

                return isDeleted;
            }

            return false;
        }


        private bool RecurringEventExists(Guid recurringEventId)
        {
            bool exists = this._recurringEvents.ContainsKey(recurringEventId);

            return exists;
        }
        

        private void PublishDomainEvent(DomainEvent domainEvent)
        {
            this._domainEvents.Add(domainEvent);
        }
    }
}