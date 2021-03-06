using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Application.Interfaces.Services;

public interface ICharacterService
{
    Task<Result> CreateCharacterAsync(CharacterId characterId, GuildId guildId, string name,
        CharacterClass characterClass,
        CharacterRace race, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result<CharacterReadModel>> GetCharacterAsync(CharacterId id, TransactionContext context,
        CancellationToken token = default);

    Task<Result> RestoreWoundsFromRestAsync(GuildId guildId, CharacterId charId, ActivityDuration activityDuration,
        TransactionContext context,
        CancellationToken token = default);

    Task<Result<IEnumerable<CharacterReadModel>>> GetAllCharactersInGuild(GuildId guildId,
        TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> EquipItemAsync(GuildId guildId, CharacterId characterId, ItemId currentItemId,
        TransactionContext dialogContext, CancellationToken cancellationToken = default);

    Task<Result> UnequipItemAsync(GuildId dialogGuildId, CharacterId characterId, ItemId currentItemId,
        TransactionContext dialogContext, CancellationToken cancellationToken = default);

    Task<Result> CraftItemAsync(GuildId guildId, CharacterId characterId, RecipeId recipeId, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> ForgeItemAsync(GuildId guildId, CharacterId characterId, EquipmentCategory category, uint level,
        List<(ItemId id, int amount)> ingredients, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> UseItemAsync(GuildId guildId, CharacterId characterId, ItemId itemId, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> SellItemAsync(GuildId guildId, CharacterId characterId, ItemId itemId, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> BuyItemAsync(GuildId dialogGuildId, CharacterId dialogCharacterId, ItemId selectedItemId,
        TransactionContext dialogContext, CancellationToken cancellationToken = default);

    Task<Result> EquipTitleAsync(GuildId guildId, CharacterId characterId, TitleId titleId, TransactionContext context,
        CancellationToken cancellationToken = default);

    Task<Result> UnequipTitleAsync(GuildId guildId, CharacterId characterId, TitleId titleId,
        TransactionContext context,
        CancellationToken cancellationToken = default);
}