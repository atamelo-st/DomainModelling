using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainModelling.ApplicationLayer.Presentation
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
