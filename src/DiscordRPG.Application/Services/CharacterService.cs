using DiscordRPG.Application.Data;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Application.Queries;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Commands;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
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

    public CharacterService(IQueryProcessor processor, ICommandBus bus, ILogger logger, IItemGenerator itemGenerator,
        IExperienceCurve experienceCurve)
    {
        this.processor = processor;
        this.bus = bus;
        this.logger = logger.WithContext(GetType());
        this.itemGenerator = itemGenerator;
        this.experienceCurve = experienceCurve;
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
            }, new List<Wound>(), new Money(10));
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

    public Task<Result> RestoreWoundsFromRestAsync(GuildId guildId, CharacterId charId,
        ActivityDuration activityDuration, TransactionContext context,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<CharacterReadModel>>> GetAllCharactersInGuild(GuildId guildId,
        TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
}