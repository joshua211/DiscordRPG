using DiscordRPG.Domain.Aggregates.Dungeon;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace DiscordRPG.Application.Models;

public class ReadModelLocator : IReadModelLocator
{
    public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        switch (domainEvent.GetAggregateEvent())
        {
            case IEntityEvent<CharacterId> ev:
                yield return ev.EntityId.Value;

                break;
            case IEntityEvent<ActivityId> ev:
                yield return ev.EntityId.Value;

                break;
            case IEntityEvent<DungeonId> ev:
                yield return ev.EntityId.Value;

                break;
            case IEntityEvent<ShopId> ev:
                yield return ev.EntityId.Value;

                break;

            default:
                yield break;
        }
    }
}