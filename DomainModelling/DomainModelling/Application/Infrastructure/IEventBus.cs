namespace DomainModelling.Application.Infrastructure
{
    public interface IEventBus
    {
        void Send<TEvent>(TEvent @event);
    }
}