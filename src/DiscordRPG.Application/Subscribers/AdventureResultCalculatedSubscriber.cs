using System.Text;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class AdventureResultCalculatedSubscriber : EventSubscriber<AdventureResultCalculated>
{
    private readonly IChannelManager channelManager;
    private readonly ILogger logger;

    public AdventureResultCalculatedSubscriber(IChannelManager channelManager, ILogger logger)
    {
        this.channelManager = channelManager;
        this.logger = logger.WithContext(GetType());
    }

    public override async Task Handle(AdventureResultCalculated domainEvent, CancellationToken cancellationToken)
    {
        logger.Debug("{Name} completed an adventure in Dungeon {Dungeon}", domainEvent.Character.CharacterName,
            domainEvent.Dungeon.Name);

        var sb = new StringBuilder();
        sb.AppendLine(
            $"<@{domainEvent.Character.UserId}> You have successfully completed the dungeon {domainEvent.Dungeon.Name}!");
        sb.AppendLine("You've found the following items:");
        foreach (var item in domainEvent.DungeonResult.Items)
            sb.AppendLine($"{item.Name}");
        sb.AppendLine("But you've also sustained some new wounds!");
        foreach (var wound in domainEvent.DungeonResult.Wounds)
            sb.AppendLine($"{wound.Description}");

        await channelManager.SendToChannelAsync(domainEvent.Dungeon.DungeonChannelId, sb.ToString());
    }
}