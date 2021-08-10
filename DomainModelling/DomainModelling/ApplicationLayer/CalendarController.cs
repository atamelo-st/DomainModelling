using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DomainModelling.DomainModel;
using DomainModelling.Common;
using DomainModelling.DataAccessLayer;


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
        // TODO: ViewModel!!!
        public IEnumerable<Event> Get(DateTime periodStart, DateTime periodEnd)
        {
            Calendar calendar = this._calendarRepo.Get(periodStart, periodEnd);

            IEnumerable<Event> calendarEvents = calendar.GetAllEvents();

            return calendarEvents;
        }

        [HttpPut]
        public void AddRegularEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime date,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            calendar.AddRegularEvent(id, title, description, date, startTime, endtime);

            this._calendarRepo.Save(calendar);

        }

        [HttpPut]
        public void UpdateRegularEvent(
            Guid id,
            [FromBody] string title,
            [FromBody] string description,
            [FromBody] DateTime date,
            [FromBody] DateTimeOffset startTime,
            [FromBody] DateTimeOffset endtime)
        {
            Calendar calendar = this._calendarRepo.Get(date, date);

            calendar.UpdateRegularEvent(id, title, description, date, startTime, endtime);

            this._calendarRepo.Save(calendar);
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
                NotFound($" Couldn't find: {parentReccurringEventId}/{date}");
            }

            this._calendarRepo.Save(calendar);

            return Ok();
        }
    }
}
