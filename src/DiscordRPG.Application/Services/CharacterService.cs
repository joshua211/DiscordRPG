using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Characters;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Services;

public class CharacterService : ApplicationService, ICharacterService
{
    private readonly IGuildService guildService;

    public CharacterService(ILogger logger, IMediator mediator, IGuildService guildService) : base(mediator, logger)
    {
        this.guildService = guildService;
    }

    public async Task<Result<Character>> CreateCharacterAsync(ulong userId, ulong guildId, string name,
        Class characterClass,
        Race race, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var character = new Character(userId, guildId, name, characterClass, race, new Level(1, 0, 100),
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

    public Task<Result> DeleteCharacterAsync(ulong userId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Character>> GetCharacterAsync(ulong userId, ulong guildId,
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