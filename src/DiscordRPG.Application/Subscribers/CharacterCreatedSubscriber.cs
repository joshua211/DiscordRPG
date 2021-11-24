using DiscordRPG.Application.Interfaces;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;
using Serilog;

namespace DiscordRPG.Application.Subscribers;

public class CharacterCreatedSubscriber : EventSubscriber<CharacterCreated>
{
    private readonly IChannelManager channelManager;
    private readonly ILogger logger;

    public CharacterCreatedSubscriber(ILogger logger, IChannelManager channelManager)
    {
        this.logger = logger;
        this.channelManager = channelManager;
    }

    public override async Task Handle(CharacterCreated domainEvent, CancellationToken cancellationToken)
    {
        logger.Information("Created character {Name}", domainEvent.Character.CharacterName);

        await channelManager.SendToGuildHallAsync(domainEvent.Character.GuildId,
            $"{domainEvent.Character.CharacterName} joined the Guild!");
    }
}