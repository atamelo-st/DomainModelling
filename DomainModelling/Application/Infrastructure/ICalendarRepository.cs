using System;
using DomainModel;

namespace Application.Infrastructure
{
    public interface ICalendarRepository
    {
        Calendar Get(DateTime periodStart, DateTime periodEnd);

        void Save(Calendar calendar);
    }
}
