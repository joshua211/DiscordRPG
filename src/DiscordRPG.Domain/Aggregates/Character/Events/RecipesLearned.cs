using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class RecipesLearned : AggregateEvent<CharacterAggregate, CharacterId>
{
    public RecipesLearned(IEnumerable<Recipe> recipes)
    {
        Recipes = recipes;
    }

    public IEnumerable<Recipe> Recipes { get; private set; }
}