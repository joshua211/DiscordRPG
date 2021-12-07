using Discord;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class CharacterDiedSubscriber : EventSubscriber<CharacterDied>
{
    private readonly IChannelManager channelManager;
    private readonly IGuildService guildService;
    private readonly ILogger logger;

    public CharacterDiedSubscriber(IChannelManager channelManager, IGuildService guildService, ILogger logger)
    {
        this.channelManager = channelManager;
        this.guildService = guildService;
        this.logger = logger.WithContext(GetType());
    }

    public override async Task Handle(CharacterDied domainEvent, CancellationToken cancellationToken)
    {
        var character = domainEvent.Character;
        var guildResult =
            await guildService.GetGuildAsync(domainEvent.Character.GuildId, cancellationToken: cancellationToken);
        if (!guildResult.WasSuccessful)
        {
            logger.Here().Warning("No guild found, cant publish death message to guild hall");
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle($"{character.CharacterName} has died!")
            .WithDescription(
                $"The Level {character.Level.CurrentLevel} {character.CharacterClass.ClassName} **{character.CharacterName}** has passed away due to a {domainEvent.FinalWound}!")
            .WithColor(Color.Red)
            .Build();
        await channelManager.SendToGuildHallAsync(guildResult.Value.ServerId, "", embed);
    }
}