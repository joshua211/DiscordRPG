using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;

namespace DiscordRPG.DiagnosticConsole.Importers;

public class AggregateImporter
{
    private readonly IAggregateStore aggregateStore;
    private readonly IEventPersistence persistence;

    public AggregateImporter(IAggregateStore aggregateStore, IEventPersistence persistence)
    {
        this.aggregateStore = aggregateStore;
        this.persistence = persistence;
    }

    public async Task<IEnumerable<string>> LoadAllAggregateIds()
    {
        var result =
            await persistence.LoadAllCommittedEvents(GlobalPosition.Start, Int32.MaxValue, CancellationToken.None);
        return result.CommittedDomainEvents.Select(c => c.AggregateId);
    }

    public async Task<TAggregate> LoadAggregate<TAggregate, TId>(TId id) where TId : IIdentity
        where TAggregate : AggregateRoot<TAggregate, TId>
    {
        var result = await aggregateStore.LoadAsync<TAggregate, TId>(id, CancellationToken.None);
        return result;
    }
}