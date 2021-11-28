using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Characters;
using MediatR;

namespace DiscordRPG.Application.Services;

public class CharacterService : ApplicationService, ICharacterService
{
    private readonly IGuildService guildService;

    public CharacterService(ILogger logger, IMediator mediator, IGuildService guildService) : base(mediator, logger)
    {
        this.guildService = guildService;
    }

    public async Task<Result<Character>> CreateCharacterAsync(DiscordId userId, Identity guildId, string name,
        int classId,
        int raceId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var character = new Character(userId, guildId, name, classId, raceId, new Level(1, 0, 100),
                new EquipmentInfo(null, null, null, null, null, null, null), new List<Item>(), new List<Wound>(), 0);

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
}