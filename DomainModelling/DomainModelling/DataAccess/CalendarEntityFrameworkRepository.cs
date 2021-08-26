using DomainModelling.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using DomainModelling.DomainModel.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DomainModelling.DataAccess
{
    public class CalendarEntityFrameworkRepository : CalendarRepositoryBase
    {
        private readonly CalendarDbContext _dbContext;

        public CalendarEntityFrameworkRepository()
        {
            this._dbContext = new CalendarDbContext();
        }
        
        
        public override void Save(Calendar calendar)
        {
             foreach (DomainEvent domainEvent in calendar.DomainEvents)
             {
                 switch (domainEvent)
                 {
                     case DomainEvent.RegularEventAdded regularEventAdded:
                     {
                         var storageItem = new StorageItem.RegularEvent(regularEventAdded.AddedEvent);
                         EntityEntry entry = this._dbContext.RegularEvents.Attach(storageItem);
                         entry.State = EntityState.Added;
                         break;
                     }

                     case DomainEvent.RegularEventUpdated regularEventUpdated:
                     {
                         var storageItem = new StorageItem.RegularEvent(regularEventUpdated.UpdatedEvent);
                         EntityEntry entry = this._dbContext.RegularEvents.Attach(storageItem);
                         entry.State = EntityState.Modified;
                         break;
                     }

                     case DomainEvent.RegularEventDeleted regularEventDeleted:
                     {
                         var storageItem = new StorageItem.RegularEvent(regularEventDeleted.DeletedEvent);
                         EntityEntry entry = this._dbContext.RegularEvents.Attach(storageItem);
                         entry.State = EntityState.Deleted;
                         break;
                     }

                     case DomainEvent.RecurringEventAdded recurringEventAdded:
                     {
                         var storageItem = new StorageItem.RecurringEvent(recurringEventAdded.AddedEvent);
                         EntityEntry entry = this._dbContext.RecurringEvents.Attach(storageItem);
                         entry.State = EntityState.Added;
                         break;
                     }

                     case DomainEvent.RecurringEventUpdated recurringEventUpdated:
                     {
                         var storageItem = new StorageItem.RecurringEvent(recurringEventUpdated.UpdatedEvent);
                         EntityEntry entry = this._dbContext.RecurringEvents.Attach(storageItem);
                         entry.State = EntityState.Modified;
                         break;
                     }

                     case DomainEvent.RecurringEventOccurrenceUpdated recurringEventOccurrenceUpdated:
                     {
                         var storageItem = new StorageItem.RecurringEventOccurrenceOverride(
                             recurringEventOccurrenceUpdated.UpdatedEventOccurrence.Parent.Id,
                             recurringEventOccurrenceUpdated.UpdatedEventOccurrence.Date,
                             recurringEventOccurrenceUpdated.UpdatedEventOccurrence.Title,
                             recurringEventOccurrenceUpdated.UpdatedEventOccurrence.Description,
                             recurringEventOccurrenceUpdated.UpdatedEventOccurrence.StartTime,
                             recurringEventOccurrenceUpdated.UpdatedEventOccurrence.EndTime);
                         
                         EntityEntry entry = this._dbContext.RecurringEventOccurrenceOverrides.Attach(storageItem);
                         entry.State = EntityState.Added;
                         break;
                     }

                     case DomainEvent.RecurringEventOccurrenceDeleted recurringEventOccurrenceDeleted:
                     {
                         var storageItem = new StorageItem.RecurringEventOccurrenceTombstone(
                             recurringEventOccurrenceDeleted.ParentRecurringEvent.Id,
                             recurringEventOccurrenceDeleted.Date);
                         
                         EntityEntry entry = this._dbContext.RecurringEventOccurrenceTombstones.Attach(storageItem);
                         entry.State = EntityState.Added;
                         break;
                     }

                     default: throw new ArgumentException($"Unknown event type: '{domainEvent.GetType().Name}'");
                 }
             }

             this._dbContext.SaveChanges();
        }
        

        protected override Calendar RehydrateInstance(DateTime periodStart, DateTime periodEnd)
        {
            Calendar instanceToRehydrate = new();
            
            RehydrateRegularEvents(this._dbContext, instanceToRehydrate, periodStart, periodEnd);
            RehydrateRecurringEvents(this._dbContext, instanceToRehydrate, periodStart, periodEnd);
            RehydrateRecurringEventOccurrenceOverrides(this._dbContext, instanceToRehydrate, periodStart, periodEnd);
            RehydrateRecurringEventOccurrenceTombstones(this._dbContext, instanceToRehydrate, periodStart, periodEnd);

            return instanceToRehydrate;
        }

        private static void RehydrateRegularEvents(CalendarDbContext dbContext, Calendar calendar, DateTime periodStart, DateTime periodEnd)
        {
            IEnumerable<RegularEvent> regularEvents =
                dbContext
                    .RegularEvents
                    .Where(regularEvent => regularEvent.Date >= periodStart && regularEvent.Date <= periodEnd)
                    .AsEnumerable()
                    .Select(evt =>
                        new RegularEvent(
                            evt.Id,
                            evt.Title,
                            evt.Description,
                            evt.Date,
                            evt.StartTime,
                            evt.EndTime)
                    );

            foreach (RegularEvent regularEvent in regularEvents)
            {
                calendar.AddRegularEvent(regularEvent);
            }
        }

        private static void RehydrateRecurringEvents(CalendarDbContext dbContext, Calendar calendar, DateTime periodStart, DateTime periodEnd)
        {
            // TODO
        }

        private static void RehydrateRecurringEventOccurrenceOverrides(CalendarDbContext dbContext, Calendar calendar,
            DateTime periodStart, DateTime periodEnd)
        {
            // TODO
        }
        
        private static void RehydrateRecurringEventOccurrenceTombstones(CalendarDbContext dbContext, Calendar calendar,
            DateTime periodStart, DateTime periodEnd)
        {
            // TODO
        }

        private class CalendarDbContext : DbContext
        {
            public DbSet<StorageItem.RegularEvent> RegularEvents { get; }
            
            public DbSet<StorageItem.RecurringEvent> RecurringEvents { get; }
            
            public DbSet<StorageItem.RecurringEventOccurrenceOverride> RecurringEventOccurrenceOverrides { get; }
            
            public DbSet<StorageItem.RecurringEventOccurrenceTombstone> RecurringEventOccurrenceTombstones { get; }

            public CalendarDbContext()
            {
                // TODO: do all the needed configuration/set-up
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);
                
                // TODO: do all the needed configuration/set-up
            }
        }
        

        private abstract class StorageItem
        {
            public class RegularEvent : StorageItem
            {
                public Guid Id { get; }
                public string Title { get; }
                public string Description { get; }
                public DateTimeOffset StartTime { get; }
                public DateTimeOffset EndTime { get; }
                public DateTime Date { get; }

                public RegularEvent(DomainModel.RegularEvent that)
                    : this(that.Id, that.Title, that.Description, that.StartTime, that.EndTime, that.Date)
                {
                }

                public RegularEvent(Guid id, string title, string description, DateTimeOffset startTime, DateTimeOffset endTime, DateTime date)
                {
                    this.Id = id;
                    this.Title = title;
                    this.Description = description;
                    this.StartTime = startTime;
                    this.EndTime = endTime;
                    this.Date = date;
                }
            }

            
            public class RecurringEvent : StorageItem
            {
                // TODO: copying constructor & EF Core constructor
                
                public RecurringEvent(DomainModel.RecurringEvent that)
                {
                }

                public RecurringEvent()
                {
                }
            }
            

            public class RecurringEventOccurrenceOverride : StorageItem
            {
                // TODO: copying constructor & EF Core constructor

                public RecurringEventOccurrenceOverride(            
                    Guid parentRecurringEventId,
                    DateTime date,
                    string newTitle,
                    string newDescription,
                    DateTimeOffset newStartTime,
                    DateTimeOffset newEndTime)
                {
                }
            }
            

            public class RecurringEventOccurrenceTombstone : StorageItem
            {
                // TODO: copying constructor & EF Core constructor
                public RecurringEventOccurrenceTombstone(Guid parentEventId, DateTime date)
                {
                    
                }
            }
        }
    }
}
