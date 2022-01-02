using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class GuildDeletedSubscriber : EventSubscriber<GuildDeleted>
{
    private readonly ICharacterService characterService;
    private readonly ILogger logger;

    public GuildDeletedSubscriber(ICharacterService characterService, ILogger logger)
    {
        this.characterService = characterService;
        this.logger = logger.WithContext(GetType());
    }

    public override async Task Handle(GuildDeleted domainEvent, CancellationToken cancellationToken)
    {
        logger.Here().Information("Handling {Name}, deleting all characters", domainEvent.GetType().Name);
        try
        {
            var charResult =
                await characterService.GetAllCharactersInGuild(domainEvent.Guild.ID,
                    cancellationToken: cancellationToken);
            foreach (var character in charResult.Value)
            {
                await characterService.DeleteAsync(character.ID, cancellationToken: cancellationToken);
            }
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed deleting all guild characters");
        }
    }
}