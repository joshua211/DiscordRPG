using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;
using Serilog;

namespace DiscordRPG.Application.Subscribers;

public class CharacterCreatedSubscriber : EventSubscriber<CharacterCreated>
{
    private readonly ILogger logger;
    private readonly IGuildMessenger messenger;

    public CharacterCreatedSubscriber(ILogger logger, IGuildMessenger messenger)
    {
        this.logger = logger;
        this.messenger = messenger;
    }

    public override async Task Handle(CharacterCreated domainEvent, CancellationToken cancellationToken)
    {
        logger.Information("Created character {Name}", domainEvent.Character.CharacterName);

        await messenger.SendToGuildHallAsync(domainEvent.Character.GuildId,
            $"{domainEvent.Character.CharacterName} joined the Guild!");
    }
}