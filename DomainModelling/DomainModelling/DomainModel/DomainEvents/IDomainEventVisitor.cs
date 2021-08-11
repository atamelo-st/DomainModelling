using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.DomainModel.DomainEvents
{
    public interface IDomainEventVisitor
    {
        //TBD: this is an alternative to swtich-style patern matching on the event type

        void Visit(DomainEvent.RegularEventAdded regularEventAdded);

        void Visit(DomainEvent.RegularEventUpdated regularEventUpdated);

        void Visit(DomainEvent.RegularEventDeleted regularEventDeleted);

        void Visit(DomainEvent.RecurringEventAdded recurringEventAdded);

        void Visit(DomainEvent.RecurringEventUpdated recurringEventUpdated);

        void Visit(DomainEvent.RecurringEventOcurrenceUpdated recurringEventOcurrenceUpdated);

        void Visit(DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted);
    }
}
