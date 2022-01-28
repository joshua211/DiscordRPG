using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.Events;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.MongoDB.ReadStores;
using EventFlow.ReadStores;

namespace DiscordRPG.Application.Models;

public class GuildReadModel : IMongoDbReadModel,
    IAmReadModelFor<GuildAggregate, GuildId, GuildCreated>,
    IAmReadModelFor<GuildAggregate, GuildId, GuildDeleted>
{
    public GuildName GuildName { get; private set; }
    public ChannelId GuildHallId { get; private set; }
    public ChannelId DungeonHallId { get; private set; }
    public ChannelId InnId { get; private set; }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, GuildCreated> domainEvent)
    {
        Id = domainEvent.AggregateEvent.Id.Value;
        GuildName = domainEvent.AggregateEvent.Name;
        GuildHallId = domainEvent.AggregateEvent.GuildHallId;
        DungeonHallId = domainEvent.AggregateEvent.DungeonHallId;
        InnId = domainEvent.AggregateEvent.InnId;
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, GuildDeleted> domainEvent)
    {
        context.MarkForDeletion();
    }

    public string Id { get; private set; }
    public long? Version { get; set; }
}