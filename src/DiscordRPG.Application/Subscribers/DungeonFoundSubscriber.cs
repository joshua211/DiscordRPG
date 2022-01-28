using Discord;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon.Events;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace DiscordRPG.Application.Subscribers;

public class DungeonFoundSubscriber : ISubscribeSynchronousTo<GuildAggregate, GuildId, DungeonAdded>
{
    private readonly IChannelManager channelManager;
    private readonly ICharacterService characterService;
    private readonly ILogger logger;

    public DungeonFoundSubscriber(IChannelManager channelManager, ILogger logger, ICharacterService characterService)
    {
        this.channelManager = channelManager;
        this.characterService = characterService;
        this.logger = logger.WithContext(GetType());
    }

    public async Task HandleAsync(IDomainEvent<GuildAggregate, GuildId, DungeonAdded> domainEvent,
        CancellationToken cancellationToken)
    {
        var context = TransactionContext.With(domainEvent.Metadata["transaction-id"]);
        logger.Context(context).Debug("Adding user to channel {Id}", domainEvent.AggregateEvent.Dungeon.Id);
        await channelManager.AddUserToThread(new ChannelId(domainEvent.AggregateEvent.Dungeon.Id.Value),
            domainEvent.AggregateEvent.CharacterId, context);

        logger.Context(context)
            .Debug("Sending Dungeon Info Embed to channel {Id}", domainEvent.AggregateEvent.Dungeon.Id);
        var dungeon = domainEvent.AggregateEvent.Dungeon;
        var embed = new EmbedBuilder()
            .WithTitle(dungeon.Name.Value)
            .WithColor(Color.Purple)
            .AddField("Rarity", dungeon.Rarity.ToString())
            .AddField("Level", dungeon.Level.Value)
            .AddField("Explorations", $"{dungeon.Explorations.Value}")
            .WithFooter(
                "This dungeon will be deleted if no explorations are left or if it has not been used for 24 hours")
            .Build();
        await channelManager.SendToChannelAsync(new ChannelId(dungeon.Id.Value), String.Empty, context, embed);

        logger.Context(context).Debug("Sending dungeon announcement to dungeon hall");
        var character =
            (await characterService.GetCharacterAsync(domainEvent.AggregateEvent.CharacterId, context,
                cancellationToken)).Value;
        embed = new EmbedBuilder().WithTitle($"New {dungeon.Rarity} Dungeon!")
            .WithDescription(
                $"{character.Name} found a new {dungeon.Rarity} Lvl {dungeon.Level} Dungeon! <#{dungeon.Id}>")
            .WithColor(Color.DarkTeal).Build();
        await channelManager.SendToDungeonHallAsync(domainEvent.AggregateIdentity, String.Empty, context, embed);
    }
}