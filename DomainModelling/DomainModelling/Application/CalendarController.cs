using System;
using System.Collections.Generic;
using System.Linq;
using DomainModelling.Application.Infrastructure;
using DomainModelling.Application.Presentation;
using DomainModelling.Common;
using DomainModelling.DomainModel;
using DomainModelling.DomainModel.DomainEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DomainModelling.Application
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarRepository _calendarRepo;
        private readonly IEventBus _eventBus;
        

        public CalendarController(ICalendarRepository calendarRepo, IEventBus eventBus)
        {
            this._calendarRepo = calendarRepo;
            this._eventBus = eventBus;
        }

        
        [HttpGet]
        public IActionResult Get(DateTime periodStart, DateTime periodEnd)
        {
            if (!Calendar.DatesWithinAllowedRange(periodStart, periodEnd))
            {
                return BadRequest($"Specified dates are too far apart.");
            }

            Calendar calendar = this._calendarRepo.Get(periodStart, periodEnd);

            IEnumerable<Event> calendarEvents = calendar.GetAllEvents();

            IEnumerable<EventViewModel> eventViewModels = calendarEvents.Select(EventToViewModel).ToList();

            var calendarViewModel = new CalendarViewModel(eventViewModels);

            return Ok(calendarViewModel);
        }

        
        [HttpPut("add-regular-event")]
        public IActionResult AddRegularEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime date,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endTime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool added = calendar.AddRegularEvent(id, title, description, date, startTime, endTime);

            if (!added)
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Event with ID={id} already exists.");
            }

            this._calendarRepo.Save(calendar);

            this.DispatchDomainEvents(calendar);

            return Ok();
        }
        

        [HttpPut("update-regular-event")]
        public IActionResult UpdateRegularEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime date,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endTime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool updated = calendar.UpdateRegularEvent(id, title, description, date, startTime, endTime);

            if (!updated)
            {
                return NotFound($"Event with ID={id} was not found.");
            }

            this._calendarRepo.Save(calendar);
            
            this.DispatchDomainEvents(calendar);

            return Ok();
        }
        

        [HttpPut("add-recurring-event")]
        public IActionResult AddRecurringEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime startDate,
            [FromBody] DateTime? endDate,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endTime,
            [FromBody] RepeatPattern.Weekly.DaysOfWeek? activeDays,
            [FromBody] int repeatInterval)
        {
            bool isDailyEvent = activeDays == null;

            RecurringEvent recurringEvent =
                isDailyEvent ?
                    RecurringEvent.Daily(
                        id,
                        title,
                        description,
                        startDate,
                        endDate ?? RecurringEvent.NoEndDate,
                        startTime,
                        endTime,
                        repeatInterval
                    ) :
                    RecurringEvent.Weekly(
                        id,
                        title,
                        description,
                        startDate,
                        endDate ?? RecurringEvent.NoEndDate,
                        startTime,
                        endTime,
                        activeDays.Value,
                        repeatInterval
                );

            Calendar calendar = this._calendarRepo.Get(startDate, startDate);

            bool added = calendar.AddRecurringEvent(recurringEvent);

            if (!added)
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Event with ID={id} already exists.");
            }

            this._calendarRepo.Save(calendar);
            
            this.DispatchDomainEvents(calendar);

            return Ok();
        }


        [HttpPut("update-occurrence")]
        public IActionResult UpdateRecurringEventOccurrence(
            [FromBody] Guid parentRecurringEventId,
            [FromBody] DateTime date,
            [FromBody] string newTitle,
            [FromBody] string newDescription,
            [FromBody] DateTimeOffset newStartTime,
            [FromBody] DateTimeOffset newEndTime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool occurenceExists = calendar.RecurringEventOccurrenceExists(parentRecurringEventId, date);

            if (!occurenceExists)
            {
                return BadRequest($"Occurrence for recurring event {parentRecurringEventId} doesn't exist for {date}.");
            }

            bool updated = calendar.UpdateRecurringEventOccurrence(parentRecurringEventId, date, newTitle, newDescription, newStartTime, newEndTime);

            if (!updated)
            {
                return NotFound();
            }

            this._calendarRepo.Save(calendar);
            
            this.DispatchDomainEvents(calendar);

            return Ok();
        }


        [HttpDelete("delete-occurrence")]
        public IActionResult DeleteRecurringEventOccurrence(Guid parentRecurringEventId, DateTime date)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool deleted = calendar.DeleteRecurringEventOccurrence(parentRecurringEventId, date);

            if (!deleted)
            {
                return NotFound($" Couldn't find: {parentRecurringEventId}/{date}");
            }

            this._calendarRepo.Save(calendar);
            
            this.DispatchDomainEvents(calendar);

            return Ok();
        }


        private static EventViewModel EventToViewModel(Event eventToConvert)
        {
            // just a safeguard in case somebody create a child of Event that we don't know of
            if (eventToConvert is not RegularEvent regularEventData)
            {
                throw new ArgumentException($"Unsupported event type {eventToConvert.GetType().FullName}");
            }

            RecurringEventData recurringEventData = null;

            if (eventToConvert is RecurringEvent.Occurrence recurringEventOccurence)
            {
                recurringEventData = new RecurringEventData(recurringEventOccurence.Parent.Id, recurringEventOccurence.Parent.RepeatPattern);
            }

            EventViewModel viewModel =
                new(
                    regularEventData.Id,
                    regularEventData.Title,
                    regularEventData.Description,
                    regularEventData.Date,
                    regularEventData.StartTime,
                    regularEventData.EndTime,
                    recurringEventData);

            return viewModel;
        }


        private void DispatchDomainEvents(Calendar calendar)
        {
            foreach (DomainEvent domainEvent in calendar.DomainEvents)
            {
                this._eventBus.Send(domainEvent);
            }
            
            calendar.AcknowledgeDomainEvents();
        }
    }
}
