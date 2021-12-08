using DiscordRPG.Application.Data;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Characters;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Progress;
using MediatR;

namespace DiscordRPG.Application.Services;

public class CharacterService : ApplicationService, ICharacterService
{
    private readonly IClassService classService;
    private readonly IExperienceCurve experienceCurve;
    private readonly IGuildService guildService;
    private readonly IRaceService raceService;

    public CharacterService(ILogger logger, IMediator mediator, IGuildService guildService, IRaceService raceService,
        IClassService classService, IExperienceCurve experienceCurve) : base(mediator, logger)
    {
        this.guildService = guildService;
        this.raceService = raceService;
        this.classService = classService;
        this.experienceCurve = experienceCurve;
    }

    public async Task<Result<Character>> CreateCharacterAsync(DiscordId userId, Identity guildId, string name,
        int classId,
        int raceId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var character = new Character(userId, guildId, name, classId, raceId,
                new Level(1, 0, experienceCurve.GetRequiredExperienceForLevel(1)),
                new EquipmentInfo(Equip.StarterWeapon, null, Equip.StarterArmor, Equip.StarterLeg, null, null, null),
                new List<Item>(), new List<Wound>(), 10);
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
            var command = new RestoreHealthCommand(characterResult.Value, amount);
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

    private float GetRecoveryAmountFromRest(ActivityDuration duration) => duration switch
    {
        ActivityDuration.Quick => 0.05f,
        ActivityDuration.Short => 0.25f,
        ActivityDuration.Medium => 0.50f,
        ActivityDuration.Long => 0.75f,
        ActivityDuration.ExtraLong => 1
    };
}