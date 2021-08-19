using System.Collections.Generic;

namespace DomainModelling.Application.Presentation
{
    public class CalendarViewModel
    {
        public IEnumerable<EventViewModel> Events { get; }

        public CalendarViewModel(IEnumerable<EventViewModel> events)
        {
            Events = events;
        }
    }
}
