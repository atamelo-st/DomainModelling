using System;
using System.Collections.Generic;
using System.Text;
using DomainModelling.ApplicationLayer.DataAccess;
using DomainModelling.DomainModel;
using DomainModelling.DomainModel.DomainEvents;

namespace DomainModelling.DataAccess
{
    public class CalendarRepository : ICalendarRepository
    {
        public Calendar Get(DateTime periodStart, DateTime periodEnd)
        {
            //TODO:
            //0. Instantiate a fresh instance of the Calendar class
            //1. Read all the event (both RegularEvent and RecurringEvent) from the database for a given timeframe
            //2. Call appropriate Calendar's Add* methods to populate the instance with data
            //3. Return the Calendar instance
            
            var calendar = new Calendar();
            
            return calendar;
        }

        public int Save(Calendar calendar)
        {
            string upsertSqlStatement = this.GetSql(calendar.DomainEvents);

            int numberOfAffectedRecords = -1;

            //TODO: execute the SQL

            return numberOfAffectedRecords;
        }

        private string GetSql(IEnumerable<DomainEvent> domainEvents)
        {
            var sb = new StringBuilder("BEGIN TRANSACTION");

            foreach (DomainEvent domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case DomainEvent.RegularEventAdded regularEventAdded:
                        {
                            ProcessDomainEvent(regularEventAdded, sb);
                            break;
                        }

                    case DomainEvent.RegularEventUpdated regularEventUpdated:
                        {
                            ProcessDomainEvent(regularEventUpdated, sb);
                            break;
                        }

                    case DomainEvent.RegularEventDeleted regularEventDeleted:
                        {
                            ProcessDomainEvent(regularEventDeleted, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventAdded recurringEventAdded:
                        {
                            ProcessDomainEvent(recurringEventAdded, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventUpdated recurringEventUpdated:
                        {
                            ProcessDomainEvent(recurringEventUpdated, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventOccurrenceUpdated recurringEventOcurrenceUpdated:
                        {
                            ProcessDomainEvent(recurringEventOcurrenceUpdated, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted:
                        {
                            ProcessDomainEvent(recurringEventOccurrenceDeleted, sb);
                            break;
                        }

                    default: throw new ArgumentException($"Unknown event type: '{domainEvent.GetType().Name}'");
                }
            }

            sb.AppendLine("END TRANSACTION");

            return sb.ToString();
        }

        private static void ProcessDomainEvent(DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventOccurrenceDeleted.GetType().Name}");
        }

        private static void ProcessDomainEvent(DomainEvent.RecurringEventOccurrenceUpdated recurringEventOccurrenceUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventOccurrenceUpdated.GetType().Name}");
        }

        private static void ProcessDomainEvent(DomainEvent.RecurringEventUpdated recurringEventUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventUpdated.GetType().Name}");
        }

        private static void ProcessDomainEvent(DomainEvent.RecurringEventAdded recurringEventAdded, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventAdded.GetType().Name}");
        }

        private static void ProcessDomainEvent(DomainEvent.RegularEventDeleted regularEventDeleted, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventDeleted.GetType().Name}");
        }

        private static void ProcessDomainEvent(DomainEvent.RegularEventUpdated regularEventUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventUpdated.GetType().Name}");
        }

        private static void ProcessDomainEvent(DomainEvent.RegularEventAdded regularEventAdded, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventAdded.GetType().Name}");
        }
    }
}
