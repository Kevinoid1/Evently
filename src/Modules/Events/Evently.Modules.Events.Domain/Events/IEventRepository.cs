namespace Evently.Modules.Events.Domain.Events;

public interface IEventRepository
{
    void Insert(Event @event);
    
    Task<Event?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
