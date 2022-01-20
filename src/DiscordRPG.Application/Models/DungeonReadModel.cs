using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Dungeon.Events;
using DiscordRPG.Domain.Entities.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;
using EventFlow.Aggregates;
using EventFlow.MongoDB.ReadStores;
using EventFlow.ReadStores;

namespace DiscordRPG.Application.Models;

public class DungeonReadModel : IMongoDbReadModel, IAmReadModelFor<GuildAggregate, GuildId, DungeonAdded>,
    IAmReadModelFor<GuildAggregate, GuildId, DungeonDeleted>,
    IAmReadModelFor<GuildAggregate, GuildId, ExplorationsDecreased>
{
    public string GuildId { get; private set; }
    public DungeonName Name { get; set; }
    public Explorations Explorations { get; set; }
    public DungeonLevel Level { get; set; }
    public Rarity Rarity { get; set; }
    public Aspect Aspect { get; set; }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, DungeonAdded> domainEvent)
    {
        var dungeon = domainEvent.AggregateEvent.Dungeon;
        Id = dungeon.Id.Value;
        GuildId = domainEvent.AggregateIdentity.Value;
        Name = dungeon.Name;
        Explorations = dungeon.Explorations;
        Level = dungeon.Level;
        Rarity = dungeon.Rarity;
        Aspect = dungeon.Aspect;
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, DungeonDeleted> domainEvent)
    {
        context.MarkForDeletion();
    }

    public void Apply(IReadModelContext context,
        IDomainEvent<GuildAggregate, GuildId, ExplorationsDecreased> domainEvent)
    {
        Explorations = Explorations.Decrease();
    }

    public string Id { get; private set; }
    public long? Version { get; set; }
}