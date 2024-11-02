using CQRS.Core.Events;
using CQRS.Core.Repositories;
using MongoDB.Driver;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _collection;

    public EventStoreRepository()
    {
        var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        var database = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));

        _collection = database.GetCollection<EventModel>(Environment.GetEnvironmentVariable("COLLECTION"));
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _collection.InsertOneAsync(@event)
            .ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _collection.Find(c =>
                c.AggregateIdentifier == aggregateId)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}