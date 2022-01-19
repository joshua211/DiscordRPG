using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class RecipeLearned : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public RecipeLearned(CharacterId entityId, Recipe recipe)
    {
        EntityId = entityId;
        Recipe = recipe;
    }

    public Recipe Recipe { get; private set; }
    public CharacterId EntityId { get; }
}