using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public class AggregateRoot
{
    protected Guid _id;
    public Guid Id => _id;
    private readonly List<BaseEvent> _uncommittedEvents = new();
    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommittedEvents() => _uncommittedEvents;
    public void MarkChangesAsCommitted() => _uncommittedEvents.Clear();

    private void ApplyChanges(BaseEvent @event, bool isNew)
    {
        var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

        if (method == null)
            throw new ArgumentNullException($"Method {@event.GetType().Name} not found!");

        method.Invoke(this, new object[] { @event });

        if (isNew)
            _uncommittedEvents.Add(@event);
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChanges(@event, true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
            ApplyChanges(@event, false);
    }
}