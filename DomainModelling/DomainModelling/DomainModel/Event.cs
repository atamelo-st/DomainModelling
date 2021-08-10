using System;
using DomainModelling.Common;


namespace DomainModelling.DomainModel
{
    public abstract class Event
    {
        public Guid Id { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTimeOffset StartTime { get; }

        //TODO: use TimeSpan instead??
        public DateTimeOffset EndTime { get; }

        protected Event(Guid id, string title, string description, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            Guard.ThrowIf(id == default, nameof(id));
            Guard.ThrowIf(string.IsNullOrWhiteSpace(title), nameof(title));
            Guard.ThrowIf(string.IsNullOrWhiteSpace(description), nameof(description));
            Guard.ThrowIf(startTime == default, nameof(startTime));
            Guard.ThrowIf(endTime == default, nameof(endTime));
            Guard.ThrowIf(startTime >= endTime, nameof(startTime) + nameof(endTime));

            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        public override bool Equals(object that)
        {
            if (object.ReferenceEquals(null, that))
            {
                return false;
            }

            if (object.ReferenceEquals(this, that))
            {
                return true;
            }

            if (that.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Event)that);
        }

        public override int GetHashCode() => HashCode.Combine(Id);

        private bool Equals(Event other) => Id == other.Id;
    }
}