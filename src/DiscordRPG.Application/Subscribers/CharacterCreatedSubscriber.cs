using Discord;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.Events;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace DiscordRPG.Application.Subscribers;

public class CharacterCreatedSubscriber : ISubscribeSynchronousTo<GuildAggregate, GuildId, CharacterCreated>
{
    private readonly IChannelManager channelManager;

    public CharacterCreatedSubscriber(IChannelManager channelManager)
    {
        this.channelManager = channelManager;
    }

    public async Task HandleAsync(IDomainEvent<GuildAggregate, GuildId, CharacterCreated> domainEvent,
        CancellationToken cancellationToken)
    {
        var character = domainEvent.AggregateEvent.Character;
        var embed = new EmbedBuilder().WithTitle("New Member!")
            .WithDescription($"The {character.Race.Name} {character.Class.Name} {character.Name} has joined the guild!")
            .WithColor(Color.Gold).Build();

        await channelManager.SendToGuildHallAsync(domainEvent.AggregateIdentity, String.Empty,
            TransactionContext.New("CharacterCreatedSubscriber"),
            embed);
    }
}