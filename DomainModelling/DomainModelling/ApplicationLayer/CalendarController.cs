using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DomainModelling.DomainModel;
using DomainModelling.Common;
using DomainModelling.DataAccessLayer;
using static DomainModelling.DomainModel.RecurringEvent;

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

            //prepare/expand events to render on the UI
            IEnumerable<Event> calendarEvents = calendar.GetAllEvents();

            return calendarEvents;
        }


        [HttpGet]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
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

            // TODO: Change to GUID !!!
            calendar.AddRegularEvent(1, title, description, date, startTime, endtime);

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

            // TODO: Change to GUID !!!
            calendar.UpdateRegularEvent(1, title, description, date, startTime, endtime);

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
            // TODO: Change to GUID !!!
            RecurringEvent recurringEvent =
                activeDays == null ?
                    RecurringEvent.Daily(
                        1,
                        title,
                        description,
                        startDate,
                        endDate ?? RecurringEvent.NoEndDate,
                        startTime,
                        endTime,
                        repeatInterval
                    ) :
                    RecurringEvent.Weekly(
                        1,
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
        public void DeleteRecurringEventOccurrence(Occurrence occurrenceToDelete)
        {
            Calendar calendar = this._calendarRepo.Get(occurrenceToDelete.Date, occurrenceToDelete.Date);

            calendar.DeleteRecurringEventOccurrence(occurrenceToDelete);

            this._calendarRepo.Save(calendar);
        }
    }
}
