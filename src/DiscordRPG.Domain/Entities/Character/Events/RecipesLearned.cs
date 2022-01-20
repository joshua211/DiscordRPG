using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class RecipesLearned : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public RecipesLearned(CharacterId entityId, IEnumerable<Recipe> recipes)
    {
        EntityId = entityId;
        Recipes = recipes;
    }

    public IEnumerable<Recipe> Recipes { get; private set; }
    public CharacterId EntityId { get; }
}