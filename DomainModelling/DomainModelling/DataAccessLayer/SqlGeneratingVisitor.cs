using DomainModelling.Common;
using DomainModelling.DomainModel.DomainEvents;
using System;
using System.Text;


namespace DomainModelling.DataAccessLayer
{
    public class SqlGeneratingVisitor : IDomainEventVisitor
    {
        public StringBuilder SqlBuffer { get; }

        public SqlGeneratingVisitor(StringBuilder sqlBuffer)
        {
            Guard.ThrowIf(sqlBuffer == null, nameof(sqlBuffer));

            SqlBuffer = sqlBuffer;
        }

        public void Visit(DomainEvent.RegularEventAdded regularEventAdded)
        {
            throw new NotImplementedException();
        }

        public void Visit(DomainEvent.RegularEventUpdated regularEventUpdated)
        {
            throw new NotImplementedException();
        }

        public void Visit(DomainEvent.RegularEventDeleted regularEventDeleted)
        {
            throw new NotImplementedException();
        }

        public void Visit(DomainEvent.RecurringEventAdded recurringEventAdded)
        {
            throw new NotImplementedException();
        }

        public void Visit(DomainEvent.RecurringEventUpdated recurringEventUpdated)
        {
            throw new NotImplementedException();
        }

        public void Visit(DomainEvent.RecurringEventOcurrenceUpdated recurringEventOcurrenceUpdated)
        {
            throw new NotImplementedException();
        }

        public void Visit(DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted)
        {
            throw new NotImplementedException();
        }
    }
}
