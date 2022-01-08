using DiscordRPG.Application.Data;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Characters;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.DomainServices.Progress;
using MediatR;

namespace DiscordRPG.Application.Services;

public class CharacterService : ApplicationService, ICharacterService
{
    private readonly IClassService classService;
    private readonly IExperienceCurve experienceCurve;
    private readonly IGuildService guildService;
    private readonly IItemGenerator itemGenerator;
    private readonly IRaceService raceService;

    public CharacterService(ILogger logger, IMediator mediator, IGuildService guildService, IRaceService raceService,
        IClassService classService, IExperienceCurve experienceCurve, IItemGenerator itemGenerator) : base(mediator,
        logger)
    {
        this.guildService = guildService;
        this.raceService = raceService;
        this.classService = classService;
        this.experienceCurve = experienceCurve;
        this.itemGenerator = itemGenerator;
    }

    public async Task<Result<Character>> CreateCharacterAsync(DiscordId userId, Identity guildId, string name,
        int classId,
        int raceId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var potion = itemGenerator.GetHealthPotion(Rarity.Common, 1);
            var armor = Equip.StarterArmor;
            var pants = Equip.StarterLeg;
            var wep = Equip.StarterWeapon;
            var character = new Character(userId, guildId, name, classId, raceId,
                new Level(1, 0, experienceCurve.GetRequiredExperienceForLevel(1)),
                new EquipmentInfo(wep, null, armor, pants, null, null),
                new List<Item>() {wep, armor, pants, potion}, new List<Wound>(), 10);
            character.ClassService = classService;
            character.RaceService = raceService;

            var result = await PublishAsync(ctx, new CreateCharacterCommand(character), cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, "Failed to create character: {Reason}", result.ErrorMessage);
                return Result<Character>.Failure("Failed to create character");
            }

            return Result<Character>.Success(character);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Character>.Failure(e.Message);
        }
    }

    public async Task<Result<Character>> GetCharacterAsync(Identity identity, TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetCharacterQuery(identity);
            var result = await ProcessAsync(ctx, query, token);

            if (result is null)
                return Result<Character>.Failure("No character found");

            return Result<Character>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Character>.Failure(e.Message);
        }
    }

    public async Task<Result<Character>> GetUsersCharacterAsync(DiscordId userId, Identity guildId,
        TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetCharacterByUserIdQuery(userId, guildId);
            var character = await ProcessAsync(ctx, query, token);

            if (character == null)
            {
                TransactionWarning(ctx, "No character with Id {Id} found", userId);
                return Result<Character>.Failure("No character found");
            }

            return Result<Character>.Success(character);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Character>.Failure(e.Message);
        }
    }

    public async Task<Result> RestoreWoundsFromRestAsync(Identity charId, ActivityDuration activityDuration,
        TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var characterResult = await GetCharacterAsync(charId, ctx, token);
            if (!characterResult.WasSuccessful)
            {
                TransactionError(ctx, characterResult.ErrorMessage);
                return Result.Failure("No character found");
            }

            var amount = GetRecoveryAmountFromRest(activityDuration);
            var maxHealth = characterResult.Value.MaxHealth;
            var amountToHeal = (int) (maxHealth * amount);

            var command = new RestoreHealthCommand(characterResult.Value, amountToHeal);
            var result = await PublishAsync(ctx, command, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, "Failed to restore health");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Character>.Failure(e.Message);
        }
    }

    public async Task<Result> UpdateEquipmentAsync(Identity identity, EquipmentInfo equipmentInfo,
        TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var command = new UpdateEquipmentCommand(identity, equipmentInfo);
            var result = await PublishAsync(ctx, command, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);
                return Result.Failure("Failed to update character equipment");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<IEnumerable<Character>>> GetAllCharactersInGuild(Identity guildId,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetAllCharactersInGuildQuery(guildId);
            var result = await ProcessAsync(ctx, query, cancellationToken);

            return Result<IEnumerable<Character>>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<IEnumerable<Character>>.Failure(e.Message);
        }
    }

    public async Task<Result> CraftItemAsync(Character character, Recipe recipe,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            recipe.Item ??= itemGenerator.GenerateFromRecipe(recipe);

            var cmd = new CraftItemCommand(character, recipe);
            var result = await PublishAsync(ctx, cmd, cancellationToken);

            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);

                return Result.Failure(result.ErrorMessage);
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result> UseItemAsync(Character character, Item item, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            Command command = null;
            if (item.Name.Contains("Health Potion"))
            {
                var hpAmount = (int) Math.Round(item.Level * 10 * (1 + (int) item.Rarity * 0.2f));
                command = new RestoreHealthCommand(character, hpAmount);
            }

            if (command is null)
            {
                TransactionError(ctx, "No Command found to handle the item {@Item}", item);
                return Result.Failure("No usage found for this item");
            }

            var result = await PublishAsync(ctx, command, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);
                return Result.Failure(result.ErrorMessage);
            }

            var removeItemCmd = new RemoveItemCommand(item.GetItemCode(), 1, character);
            result = await PublishAsync(ctx, removeItemCmd, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);
                return Result.Failure(result.ErrorMessage);
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result> DeleteAsync(Identity characterId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var cmd = new DeleteCharacterCommand(characterId);
            var result = await PublishAsync(ctx, cmd, cancellationToken);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, result.ErrorMessage);
                return Result.Failure(result.ErrorMessage);
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    private float GetRecoveryAmountFromRest(ActivityDuration duration) => duration switch
    {
        ActivityDuration.Quick => 0.05f,
        ActivityDuration.Short => 0.25f,
        ActivityDuration.Medium => 0.50f,
        ActivityDuration.Long => 0.75f,
        ActivityDuration.ExtraLong => 1
    };
}