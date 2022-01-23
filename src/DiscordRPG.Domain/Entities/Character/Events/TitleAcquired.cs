using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class TitleAcquired : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public TitleAcquired(CharacterId characterId, Title title)
    {
        Title = title;
        EntityId = characterId;
    }

    public Title Title { get; private set; }

    public CharacterId EntityId { get; }
}