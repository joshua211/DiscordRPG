using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Services;

public class CharacterService : ApplicationService, ICharacterService
{
    public CharacterService(ILogger logger, IMediator mediator) : base(mediator, logger)
    {
    }

    public async Task<Result<Character>> CreateCharacterAsync(ulong userId, ulong guildId, string name,
        Class characterClass,
        Race race,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var character = new Character(userId, guildId, name, characterClass, race, new Level(1, 0, 100),
                new EquipmentInfo(null, null, null, null, null, null, null), new List<Item>(), new List<Wound>(), 0);

            await PublishAsync(ctx, new CreateCharacterCommand(character), cancellationToken);

            return Result<Character>.Success(character);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Character>.Failure(e.Message);
        }
    }

    public Task<Result> DeleteCharacterAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Character>> GetCharacterAsync(ulong userId, ulong guildId,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var query = new GetCharacterByUserIdQuery(userId, guildId);
            var character = await ProcessAsync(ctx, query, token);

            if (character == null)
            {
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