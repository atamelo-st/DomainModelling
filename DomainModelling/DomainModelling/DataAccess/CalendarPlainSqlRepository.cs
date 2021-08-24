using System;
using System.Collections.Generic;
using System.Text;
using DomainModelling.Application.Infrastructure;
using DomainModelling.DomainModel;
using DomainModelling.DomainModel.DomainEvents;

namespace DomainModelling.DataAccess
{
    public class CalendarPlainSqlRepository : CalendarRepositoryBase
    {
        public override void Save(Calendar calendar)
        {
            string upsertSqlStatement = this.GetSql(calendar.DomainEvents);

            //TODO: execute the SQL
        }


        protected override IEnumerable<CalendarStorageItem> GetCalendarData(DateTime periodStart, DateTime periodEnd)
        {
            throw new NotImplementedException();
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
                            AppendSqlForDomainEvent(regularEventAdded, sb);
                            break;
                        }

                    case DomainEvent.RegularEventUpdated regularEventUpdated:
                        {
                            AppendSqlForDomainEvent(regularEventUpdated, sb);
                            break;
                        }

                    case DomainEvent.RegularEventDeleted regularEventDeleted:
                        {
                            AppendSqlForDomainEvent(regularEventDeleted, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventAdded recurringEventAdded:
                        {
                            AppendSqlForDomainEvent(recurringEventAdded, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventUpdated recurringEventUpdated:
                        {
                            AppendSqlForDomainEvent(recurringEventUpdated, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventOccurrenceUpdated recurringEventOcurrenceUpdated:
                        {
                            AppendSqlForDomainEvent(recurringEventOcurrenceUpdated, sb);
                            break;
                        }

                    case DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted:
                        {
                            AppendSqlForDomainEvent(recurringEventOccurrenceDeleted, sb);
                            break;
                        }

                    default: throw new ArgumentException($"Unknown event type: '{domainEvent.GetType().Name}'");
                }
            }

            sb.AppendLine("END TRANSACTION");

            return sb.ToString();
        }

        private static void AppendSqlForDomainEvent(DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventOccurrenceDeleted.GetType().Name}");
        }

        private static void AppendSqlForDomainEvent(DomainEvent.RecurringEventOccurrenceUpdated recurringEventOccurrenceUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventOccurrenceUpdated.GetType().Name}");
        }

        private static void AppendSqlForDomainEvent(DomainEvent.RecurringEventUpdated recurringEventUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventUpdated.GetType().Name}");
        }

        private static void AppendSqlForDomainEvent(DomainEvent.RecurringEventAdded recurringEventAdded, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {recurringEventAdded.GetType().Name}");
        }

        private static void AppendSqlForDomainEvent(DomainEvent.RegularEventDeleted regularEventDeleted, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventDeleted.GetType().Name}");
        }

        private static void AppendSqlForDomainEvent(DomainEvent.RegularEventUpdated regularEventUpdated, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventUpdated.GetType().Name}");
        }

        private static void AppendSqlForDomainEvent(DomainEvent.RegularEventAdded regularEventAdded, StringBuilder sb)
        {
            sb.AppendLine($"SQL Imitation for {regularEventAdded.GetType().Name}");
        }

    }
}
