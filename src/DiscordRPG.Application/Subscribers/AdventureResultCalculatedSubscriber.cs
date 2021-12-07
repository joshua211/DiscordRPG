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
        var character = domainEvent.Character;
        var dungeon = domainEvent.Dungeon;
        var sb = new StringBuilder();
        if (domainEvent.WoundsResult.HasDied)
        {
            sb.AppendLine(
                $"<@{character.UserId}> it seems like you were no match for the level {dungeon.DungeonLevel} dungeon **{dungeon.Name}**");
            sb.AppendLine($"Sadly, you have died due to a {domainEvent.WoundsResult.FinalWound}");
        }
        else
        {
            sb.AppendLine(
                $"<@{character.UserId}> You've completed the level {dungeon.DungeonLevel} dungeon **{dungeon.Name}**!");
            sb.Append($"You gained {domainEvent.ExperienceResult.TotalExperienceGained} exp");
            if (domainEvent.ExperienceResult.TotalLevelsGained > 0)
                sb.Append($" and leveled up to level {domainEvent.ExperienceResult.NewLevel}");
            sb.AppendLine("!");
            sb.AppendLine();
            if (domainEvent.ItemResult.ItemsGained.Any())
            {
                sb.AppendLine("In the dungeon you found the following items:");
                foreach (var item in domainEvent.ItemResult.ItemsGained)
                {
                    sb.AppendLine($" - _{item}_");
                }

                sb.AppendLine();
            }

            if (domainEvent.WoundsResult.WoundsGained.Any())
            {
                sb.AppendLine("But you also sustained these wounds:");
                foreach (var wound in domainEvent.WoundsResult.WoundsGained)
                {
                    sb.AppendLine($" - _{wound}_");
                }
            }
        }

        await channelManager.SendToChannelAsync(dungeon.DungeonChannelId, sb.ToString());
    }
}