using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModelling.DomainModel
{
    public class Calendar
    {
        //NOTE: just for a quick and dirty implementation;
        //interval trees are needed for a prod-ready code
        private readonly HashSet<RegularEvent> _regularEvents;
        private readonly HashSet<RecurringEvent> _recurringEvents;
        //NOTE: RecurringEvent(aka parent) -> occurence overrides
        private readonly IDictionary<RecurringEvent, IList<RecurringEvent.Occurrence>> _recurringOccurrencesOverrides;
        private readonly IDictionary<Guid, HashSet<DateTime>> _recurringOccurrencesTombstones;

        public Calendar()
        {
            this._regularEvents = new HashSet<RegularEvent>();
            this._recurringEvents = new HashSet<RecurringEvent>();
            // TODO: change to RecurringEvent.Id vs RecurringEvent
            this._recurringOccurrencesOverrides = new Dictionary<RecurringEvent, IList<RecurringEvent.Occurrence>>();
            this._recurringOccurrencesTombstones = new Dictionary<Guid, HashSet<DateTime>>();
        }

        public bool AddRegularEvent(RegularEvent regularEvent)
        {
            bool added = this._regularEvents.Add(regularEvent);

            return added;
        }

        public bool UpdateRegularEvent(RegularEvent regularEvent)
        {
            //NOTE: since we've provided custom equality logic, only event.Id is taken into account
            //when event is searched/deleted.
            //So to update an already existing event, just pass in a new instance with the same Id and updated fields
            if (!this._regularEvents.Remove(regularEvent))
            {
                return false;
            }

            bool updated = this._regularEvents.Add(regularEvent);

            return updated;
        }

        public bool DeleteRegularEvent(RegularEvent regularEvent)
        {
            bool deleted = this._regularEvents.Remove(regularEvent);

            return deleted;
        }

        public bool AddRecurringEvent(RecurringEvent recurringEvent)
        {
            bool added = this._recurringEvents.Add(recurringEvent);

            return added;
        }

        public bool UpdateRecurringEvent(RecurringEvent recurringEvent)
        {
            if (!this._recurringEvents.Remove(recurringEvent))
            {
                return false;
            }

            bool updated = this._recurringEvents.Add(recurringEvent);

            return updated;
        }

        public bool UpdateRecurringEventOccurrence(RecurringEvent.Occurrence occurence)
        {
            //NOTE: I've chosen this semantics as the simplest - can't update a deleted occurence.
            //As an alternative, we could consider possibility of resurrecting an occurence -
            //e.g. if it was deleted by mistake, updating a deleted item would 'bring it back'..
            if (this.IsOccurrenceDeleted(occurence))
            {
                return false;
            }

            //Recurrent event has overrides?
            if (this._recurringOccurrencesOverrides.TryGetValue(occurence.Parent, out var occurrenceOverrides))
            {

                (RecurringEvent.Occurrence @override, int index)? found = this.FindOccurrenceOverride(occurence);

                if (found != null)
                {
                    //If there is an old override - remove it
                    occurrenceOverrides.RemoveAt(found.Value.index);
                }
            }
            else //No overrides - create it
            {
                occurrenceOverrides = new List<RecurringEvent.Occurrence>();
                this._recurringOccurrencesOverrides.Add(occurence.Parent, occurrenceOverrides);
            }

            //add submitted override
            occurrenceOverrides.Add(occurence);

            return true;
        }
        
        // TODO: add this API
        public bool UpdateRecurringEventOccurrence(
            Guid parentRecurringEventId,
            string newTitle,
            string newDescription,
            DateTime newDate,
            DateTimeOffset newStartTime,
            DateTimeOffset newEndTime)
        {
            RecurringEvent parentRecurringEvent = null; //TODO: this.__recurringEvents.TryGetValue(...)

            var updatedOccurence =
                new RecurringEvent.Occurrence(
                    parentRecurringEvent,
                    newTitle,
                    newDescription,
                    newDate,
                    newStartTime,
                    newEndTime
                );

            //add submitted override
            //occurrenceOverrides.Add(updatedOccurence);

            return true;
        }

        public bool DeleteRecurringEventOccurrence(RecurringEvent.Occurrence occurence)
        {
            bool deleted = this.DeleteRecurringEventOccurrence(occurence.Parent.Id, occurence.Date);

            return deleted;
        }

 
        public bool DeleteRecurringEventOccurrence(Guid parentReccurreingEventId, DateTime date)
        {
            if (!this._recurringOccurrencesTombstones.TryGetValue(parentReccurreingEventId, out var tombstones))
            {
                tombstones = new HashSet<DateTime>();

                this._recurringOccurrencesTombstones.Add(parentReccurreingEventId, tombstones);
            }

            bool deleted = tombstones.Add(date);

            return deleted;
        }


        public IEnumerable<Event> GetAllEvents()
        {
            return this.GetEvents(DateTime.MinValue, DateTime.MaxValue);
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

            foreach (RecurringEvent recurringEvent in this._recurringEvents)
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

        private RecurringEvent.Occurrence ResolveOccurenceOverride(RecurringEvent.Occurrence recurringOccurrence)
        {
            (RecurringEvent.Occurrence @override, int index)? found = this.FindOccurrenceOverride(recurringOccurrence);

            RecurringEvent.Occurrence resolvedOverride = found?.@override ?? recurringOccurrence;

            return resolvedOverride;
        }

        //NOTE: brute-force search for demo only :)
        private (RecurringEvent.Occurrence @override, int index)? FindOccurrenceOverride(RecurringEvent.Occurrence recurringOccurrence)
        {
            //Recurrent event has overrides?
            if (this._recurringOccurrencesOverrides.TryGetValue(recurringOccurrence.Parent, out var occurrenceOverrides))
            {
                //NOTE: the compound primary key for an override occurence
                // is (Parent, Date) - i.e. for a given recurring event there could be only one
                // occurence with a unique combination of those/
                (RecurringEvent.Occurrence occurrenceOverride, int overrideItemindex)? found =
                    occurrenceOverrides
                        //Are there overrides for the occurrence's date?
                        .Where(ovrd => ovrd.Date == recurringOccurrence.Date)
                        .Select((ovrd, index) => ((RecurringEvent.Occurrence, int)?)(ovrd, index))
                        .SingleOrDefault();

                return found;
            }

            return null;
        }

        private bool IsOccurrenceDeleted(RecurringEvent.Occurrence occurence)
        {
            if (this._recurringOccurrencesTombstones.TryGetValue(occurence.Parent.Id, out var dates))
            {
                bool isDeleted = dates.Contains(occurence.Date);

                return isDeleted;
            }

            return false;
        }
    }
}