using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands;
using MediatR;

namespace DiscordRPG.Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IMediator mediator;

    public CharacterService(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<Result<Character>> CreateCharacterAsync(ulong userId, string name, Class characterClass,
        Race race,
        CancellationToken cancellationToken = default)
    {
        var character = new Character(userId, name, characterClass, race, new Level(1, 0, 100),
            new EquipmentInfo(null, null, null, null, null, null, null), new List<Item>(), new List<Wound>(), 0);

        await mediator.Publish(new CreateCharacterCommand(character), cancellationToken);

        return Result<Character>.Success(character);
    }

    public Task<Result> DeleteCharacterAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Character>> GetCharacterAsync(ulong userId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}