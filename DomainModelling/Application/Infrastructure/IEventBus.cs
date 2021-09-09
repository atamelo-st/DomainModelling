namespace Application.Infrastructure
{
    public interface IEventBus
    {
        void Send<TEvent>(TEvent @event);
    }
}