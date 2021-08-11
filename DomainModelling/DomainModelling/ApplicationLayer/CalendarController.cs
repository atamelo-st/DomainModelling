using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DomainModelling.DomainModel;
using DomainModelling.Common;
using DomainModelling.DataAccessLayer;
using DomainModelling.ApplicationLayer.Presentation;
using Microsoft.AspNetCore.Http;

namespace DomainModelling.ApplicationLayer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarRepository _calendarRepo;

        public CalendarController(ICalendarRepository calendarRepo)
        {
            this._calendarRepo = calendarRepo;
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
            [FromBody] DateTimeOffset endtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool added = calendar.AddRegularEvent(id, title, description, date, startTime, endtime);

            if (!added)
            {
                return StatusCode(StatusCodes.Status409Conflict, $"Event with ID={id} already exists.");
            }

            int recordsSaved = this._calendarRepo.Save(calendar);

            return Ok(recordsSaved);
        }

        [HttpPut("update-regular-event")]
        public IActionResult UpdateRegularEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime date,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool updated = calendar.UpdateRegularEvent(id, title, description, date, startTime, endtime);

            if (!updated)
            {
                return NotFound($"Event with ID={id} was not found.");
            }

            int recordsSaved = this._calendarRepo.Save(calendar);

            return Ok(recordsSaved);
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

            int recordsSaved = this._calendarRepo.Save(calendar);

            return Ok(recordsSaved);
        }


        [HttpPut("update-occurrence")]
        public IActionResult UpdateRecurringEventOccurrence(
            [FromBody] Guid parentReccurringEventId,
            [FromBody] DateTime date,
            [FromBody] string newTitle,
            [FromBody] string newDescription,
            [FromBody] DateTimeOffset newStartTime,
            [FromBody] DateTimeOffset newEndtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool updated = calendar.UpdateRecurringEventOccurrence(parentReccurringEventId, date, newTitle, newDescription, newStartTime, newEndtime);

            if (!updated)
            {
                return NotFound();
            }

            int recordsSaved = this._calendarRepo.Save(calendar);

            return Ok(recordsSaved);
        }


        [HttpDelete("delete-occurrence")]
        public IActionResult DeleteRecurringEventOccurrence(Guid parentReccurringEventId, DateTime date)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            bool deleted = calendar.DeleteRecurringEventOccurrence(parentReccurringEventId, date);

            if (!deleted)
            {
                return NotFound($" Couldn't find: {parentReccurringEventId}/{date}");
            }

            this._calendarRepo.Save(calendar);

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
    }
}
