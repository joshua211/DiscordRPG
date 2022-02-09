using DiscordRPG.Application.Data;
using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Application.Queries;
using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Character.Commands;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Enums;
using DiscordRPG.Domain.Services;
using EventFlow;
using EventFlow.Queries;

namespace DiscordRPG.Application.Services;

public class CharacterService : ICharacterService
{
    private readonly ICommandBus bus;
    private readonly IExperienceCurve experienceCurve;
    private readonly IItemGenerator itemGenerator;
    private readonly ILogger logger;
    private readonly IQueryProcessor processor;
    private readonly RecipeGenerator recipeGenerator;
    private readonly IWoundReducer woundReducer;

    public CharacterService(IQueryProcessor processor, ICommandBus bus, ILogger logger, IItemGenerator itemGenerator,
        IExperienceCurve experienceCurve, IWoundReducer woundReducer, RecipeGenerator recipeGenerator)
    {
        this.processor = processor;
        this.bus = bus;
        this.logger = logger.WithContext(GetType());
        this.itemGenerator = itemGenerator;
        this.experienceCurve = experienceCurve;
        this.woundReducer = woundReducer;
        this.recipeGenerator = recipeGenerator;
    }

    public async Task<Result> CreateCharacterAsync(CharacterId characterId, GuildId guildId, string name,
        CharacterClass characterClass,
        CharacterRace race, TransactionContext context, CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Creating Character {Name}", name);
        var character = new Character(characterId, characterClass, race, new CharacterName(name),
            new Level(1, 0, experienceCurve.GetRequiredExperienceForLevel(1)),
            new List<Item>
            {
                Equip.StarterArmor, Equip.StarterLeg, Equip.StarterWeapon, Equip.StarterAmulet,
                itemGenerator.GetHealthPotion(Rarity.Common, 1)
            }, new List<Wound>(), new Money(10), recipeGenerator.GenerateRecipesForLevel(1).ToList(), new List<Title>
            {
                new(TitleId.New, Rarity.Legendary, "Alpha Tester",
                    new[] {new StatusEffect(StatusEffectType.ExpBoost, 0.1f, "Experienced Player")}, true)
            });

        var cmd = new CreateCharacterCommand(guildId, character, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to create Character {@Character}", character);
            return Result.Failure("Failed to create Character");
        }

        return Result.Success();
    }

    public async Task<Result<CharacterReadModel>> GetCharacterAsync(CharacterId id, TransactionContext context,
        CancellationToken token = default)
    {
        logger.Context(context).Information("Querying Character with Id {Id}", id.Value);
        var query = new GetCharacterQuery(id);
        var result = await processor.ProcessAsync(query, token);
        logger.Context(context).Information("Found Character {Name}", result?.Name.Value);

        return Result<CharacterReadModel>.Success(result);
    }

    public async Task<Result> RestoreWoundsFromRestAsync(GuildId guildId, CharacterId charId,
        ActivityDuration activityDuration, TransactionContext context,
        CancellationToken token = default)
    {
        var percent = activityDuration switch
        {
            ActivityDuration.Quick => 0.05f,
            ActivityDuration.Short => 0.25f,
            ActivityDuration.Medium => 0.5f,
            ActivityDuration.Long => 0.75f
        };

        var character = (await GetCharacterAsync(charId, context, token)).Value;
        var hpToHeal = (int) (character.MaxHealth * percent);
        var newWounds = woundReducer.ReduceDamageFromWounds(character.Wounds, hpToHeal).ToList();

        var cmd = new ChangeWoundsCommand(guildId, charId, newWounds, context);
        var result = await bus.PublishAsync(cmd, token);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to restore wounds");

            return Result.Failure("Failed to restore wounds");
        }

        return Result.Success();
    }

    public async Task<Result<IEnumerable<CharacterReadModel>>> GetAllCharactersInGuild(GuildId guildId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Querying all Characters of Guild {Id}", guildId.Value);
        var query = new GetAllGuildCharacters(guildId);
        var result = await processor.ProcessAsync(query, cancellationToken);

        logger.Context(context).Debug("Found {Count} characters", result.Count());
        return Result<IEnumerable<CharacterReadModel>>.Success(result);
    }

    public async Task<Result> EquipItemAsync(GuildId guildId, CharacterId characterId, ItemId itemId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Equipping Item {id} for Character {CharId}", itemId, characterId);
        var cmd = new EquipItemCommand(guildId, characterId, itemId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to equip item");
            return Result.Failure("Failed to equip item!");
        }

        return Result.Success();
    }

    public async Task<Result> UnequipItemAsync(GuildId guildId, CharacterId characterId, ItemId itemId,
        TransactionContext context, CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Unequipping Item {id} for Character {CharId}", itemId, characterId);
        var cmd = new UnequipItemCommand(guildId, characterId, itemId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to unequip item");
            return Result.Failure("Failed to unequip item!");
        }

        return Result.Success();
    }

    public async Task<Result> CraftItemAsync(GuildId guildId, CharacterId characterId, RecipeId recipeId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Crafting recipe {RecId} for character {CharId}", recipeId, characterId);
        var cmd = new CraftItemCommand(guildId, characterId, recipeId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to craft item");
            return Result.Failure("Failed to craft item!");
        }

        return Result.Success();
    }

    public async Task<Result> ForgeItemAsync(GuildId guildId, CharacterId characterId, EquipmentCategory category,
        uint level,
        List<(ItemId id, int amount)> ingredients, TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Forging item for character {CharId}", characterId);
        var cmd = new ForgeItemCommand(guildId, characterId, ingredients, context, category, level);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to forge item");

            return Result.Failure("Failed to forge item");
        }

        return Result.Success();
    }

    public async Task<Result> UseItemAsync(GuildId guildId, CharacterId characterId, ItemId itemId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Using Item {ItemId} for character {CharacterId}", itemId, characterId);
        var cmd = new UseItemCommand(guildId, characterId, itemId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context)
                .Error("Failed to use item {ItemId} for character {CharacterId}", itemId, characterId);

            return Result.Failure("Failed to use item!");
        }

        return Result.Success();
    }

    public async Task<Result> SellItemAsync(GuildId guildId, CharacterId characterId, ItemId itemId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        var cmd = new SellItemCommand(guildId, characterId, itemId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to sell item {Id}", itemId);

            return Result.Failure("Failed to sell item!");
        }

        return Result.Success();
    }

    public async Task<Result> BuyItemAsync(GuildId guildId, CharacterId characterId, ItemId itemId,
        TransactionContext context, CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Buying Item {ItemId} for character {CharacterId}", itemId, characterId);
        var cmd = new BuyItemCommand(guildId, characterId, itemId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to buy item");
            return Result.Failure("Failed to buy item");
        }

        return Result.Success();
    }

    public async Task<Result> EquipTitleAsync(GuildId guildId, CharacterId characterId, TitleId titleId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Equipping title {TitleId} for Character {CharId}", titleId, characterId);
        var cmd = new EquipTitleCommand(guildId, titleId, characterId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to equip title");
            return Result.Failure("Failed to equip title");
        }

        return Result.Success();
    }

    public async Task<Result> UnequipTitleAsync(GuildId guildId, CharacterId characterId, TitleId titleId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Unequipping title {TitleId} for Character {CharId}", titleId, characterId);
        var cmd = new UnequipTitleCommand(guildId, titleId, characterId, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to unequip title");
            return Result.Failure("Failed to unequip title");
        }

        return Result.Success();
    }
}