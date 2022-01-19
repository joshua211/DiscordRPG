using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Domain.DomainServices;

public interface ICraftingService
{
    void CraftItem(GuildAggregate aggregate, CharacterId characterId, RecipeId recipeId, TransactionContext context);
}