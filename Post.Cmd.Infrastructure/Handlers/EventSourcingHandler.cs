using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;

    public EventSourcingHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task SaveAsync(AggregateRoot aggregateRoot)
    {
        await _eventStore.SaveEventAsync(
            aggregateRoot.Id,
            aggregateRoot.GetUncommittedEvents(),
            aggregateRoot.Version);
        
        aggregateRoot.MarkChangesAsCommitted();
    }

    public async Task<PostAggregate> GetByIdAsync(Guid id)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(id);

        if (events == null || !events.Any())
            return aggregate;

        aggregate.ReplayEvents(events);
        aggregate.Version = events
            .Select(x => x.Version)
            .Max();
        
        return aggregate;
    }
}