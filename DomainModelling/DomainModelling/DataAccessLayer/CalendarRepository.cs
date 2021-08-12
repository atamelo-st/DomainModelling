using System;
using System.Collections.Generic;
using System.Text;
using DomainModelling.DomainModel;
using DomainModelling.DomainModel.DomainEvents;


namespace DomainModelling.DataAccessLayer
{
    public class CalendarRepository : ICalendarRepository
    {
        public Calendar Get(DateTime periodStart, DateTime periodEnd)
        {
            throw new NotImplementedException();
        }

        public int Save(Calendar calendar)
        {
            string upsertSqlStatement = this.GetSql(calendar.DomainEvents);

            //TODO: execute the SQL

            return -1;
        }

        private string GetSql(IEnumerable<DomainEvent> domainEvents)
        {
            var sb = new StringBuilder("BRGIN TRANSACTION");

            foreach (DomainEvent domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case DomainEvent.RegularEventAdded regularEventAdded:
                        {
                            this.ProcessDomainEvent(regularEventAdded, sb);
                            break;
                        }

                    case DomainEvent.RegularEventUpdated regularEventUpdated:
                        {
                            this.ProcessDomainEvent(regularEventUpdated, sb);
                            break;
                        }

                    case DomainEvent.RegularEventDeleted regularEventDeleted:
                        {
                            this.ProcessDomainEvent(regularEventDeleted, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventAdded recurringEventAdded:
                        {
                            this.ProcessDomainEvent(recurringEventAdded, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventUpdated recurringEventUpdated:
                        {
                            this.ProcessDomainEvent(recurringEventUpdated, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventOccurrenceUpdated recurringEventOcurrenceUpdated:
                        {
                            this.ProcessDomainEvent(recurringEventOcurrenceUpdated, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted:
                        {
                            this.ProcessDomainEvent(recurringEventOccurrenceDeleted, sb);
                            break;
                        }

                    default: throw new ArgumentException($"Unknown event type: '{domainEvent.GetType().Name}'");
                }
            }

            sb.AppendLine("END TRANSACTION");

            return sb.ToString();
        }

        private void ProcessDomainEvent(DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventOccurrenceDeleted.GetType().Name}");
        }

        private void ProcessDomainEvent(DomainEvent.RecurringEventOccurrenceUpdated recurringEventOccurrenceUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventOccurrenceUpdated.GetType().Name}");
        }

        private void ProcessDomainEvent(DomainEvent.RecurringEventUpdated recurringEventUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventUpdated.GetType().Name}");
        }

        private void ProcessDomainEvent(DomainEvent.RecurringEventAdded recurringEventAdded, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventAdded.GetType().Name}");
        }

        private void ProcessDomainEvent(DomainEvent.RegularEventDeleted regularEventDeleted, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventDeleted.GetType().Name}");
        }

        private void ProcessDomainEvent(DomainEvent.RegularEventUpdated regularEventUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventUpdated.GetType().Name}");
        }

        private void ProcessDomainEvent(DomainEvent.RegularEventAdded regularEventAdded, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventAdded.GetType().Name}");
        }
    }
}
