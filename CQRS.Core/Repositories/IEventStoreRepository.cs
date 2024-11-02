using CQRS.Core.Events;

namespace CQRS.Core.Repositories;

public interface IEventStoreRepository
{
    Task SaveAsync(EventModel @event);
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
}