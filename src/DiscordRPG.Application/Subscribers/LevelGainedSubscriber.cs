using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class LevelGainedSubscriber : IDomainEventSubscriber<LevelGained>
{
    private readonly ICharacterService characterService;

    public LevelGainedSubscriber(ICharacterService characterService)
    {
        this.characterService = characterService;
    }

    public async Task Handle(LevelGained notification, CancellationToken cancellationToken)
    {
        await characterService.RestoreWoundsFromRestAsync(notification.CharId, ActivityDuration.Medium,
            token: cancellationToken);
    }
}