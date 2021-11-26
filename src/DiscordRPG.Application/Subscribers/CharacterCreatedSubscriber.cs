using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;
using Serilog;

namespace DiscordRPG.Application.Subscribers;

public class CharacterCreatedSubscriber : EventSubscriber<CharacterCreated>
{
    private readonly IChannelManager channelManager;
    private readonly IGuildService guildService;
    private readonly ILogger logger;

    public CharacterCreatedSubscriber(ILogger logger, IChannelManager channelManager, IGuildService guildService)
    {
        this.logger = logger;
        this.channelManager = channelManager;
        this.guildService = guildService;
    }

    public override async Task Handle(CharacterCreated domainEvent, CancellationToken cancellationToken)
    {
        var guildResult =
            await guildService.GetGuildAsync(domainEvent.Character.GuildId, cancellationToken: cancellationToken);
        if (!guildResult.WasSuccessful)
        {
            logger.Warning("No guild found to complete the event {Name}", domainEvent.GetType().Name);
            return;
        }

        await channelManager.SendToGuildHallAsync(guildResult.Value.ServerId,
            $"{domainEvent.Character.CharacterName} joined the Guild!");
    }
}