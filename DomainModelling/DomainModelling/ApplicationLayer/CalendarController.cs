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
            Calendar calendar = this._calendarRepo.Get(periodStart, periodEnd);

            IEnumerable<Event> calendarEvents = calendar.GetAllEvents();

            IEnumerable<EventViewModel> eventViewModels = calendarEvents.Select(EventToViewModel);

            var calendarViewModel = new CalendarViewModel(eventViewModels);

            return Ok(calendarViewModel);
        }

        [HttpPut]
        public IActionResult AddRegularEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime date,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            calendar.AddRegularEvent(id, title, description, date, startTime, endtime);

            int recordsSaved = this._calendarRepo.Save(calendar);

            return Ok(recordsSaved);
        }

        [HttpPut]
        public IActionResult UpdateRegularEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime date,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            calendar.UpdateRegularEvent(id, title, description, date, startTime, endtime);

            int recordsSaved = this._calendarRepo.Save(calendar);

            return Ok(recordsSaved);
        }

        [HttpPut]
        public void AddRecurringEvent(
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

            calendar.AddRecurringEvent(recurringEvent);

            this._calendarRepo.Save(calendar);
        }

        [HttpPut]
        public void UpdateRecurringEventOccurrence(
            [FromBody] RecurringEvent parent,
            [FromBody] DateTime date,
            [FromBody] string newTitle,
            [FromBody] string newDescription,
            [FromBody] DateTimeOffset newStartTime,
            [FromBody] DateTimeOffset newEndtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            // TODO: Change to ID vs parent !!!
            calendar.UpdateRecurringEventOccurrence(parent, newTitle, newDescription, date, newStartTime, newEndtime);

            this._calendarRepo.Save(calendar);
        }


        [HttpDelete]
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
                new EventViewModel(
                        regularEventData.Id,
                        regularEventData.Title,
                        regularEventData.Description,
                        regularEventData.Date,
                        regularEventData.StartTime,
                        regularEventData.EndTime,
                        recurringEventData
                );

            return viewModel;
        }
    }
}
