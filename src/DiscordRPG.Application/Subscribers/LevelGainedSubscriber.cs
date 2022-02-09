using Discord;
using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Character.Commands;
using DiscordRPG.Domain.Aggregates.Character.Events;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Subscribers;

namespace DiscordRPG.Application.Subscribers;

public class LevelGainedSubscriber : ISubscribeSynchronousTo<GuildAggregate, GuildId, LevelGained>
{
    private readonly ICommandBus bus;
    private readonly IChannelManager channelManager;
    private readonly ICharacterService characterService;
    private readonly RecipeGenerator recipeGenerator;

    public LevelGainedSubscriber(RecipeGenerator recipeGenerator, ICommandBus bus, ICharacterService characterService,
        IChannelManager channelManager)
    {
        this.recipeGenerator = recipeGenerator;
        this.bus = bus;
        this.characterService = characterService;
        this.channelManager = channelManager;
    }

    public async Task HandleAsync(IDomainEvent<GuildAggregate, GuildId, LevelGained> domainEvent,
        CancellationToken cancellationToken)
    {
        var context = TransactionContext.With(domainEvent.Metadata["transaction-id"]);
        var ev = domainEvent.AggregateEvent;
        if (ev.NewLevel.CurrentLevel <= ev.OldLevel.CurrentLevel)
            return;

        var recipes = recipeGenerator.GenerateRecipesForLevel(ev.NewLevel.CurrentLevel);
        var cmd = new LearnRecipesCommand(domainEvent.AggregateIdentity, ev.EntityId, recipes,
            context);
        await bus.PublishAsync(cmd, cancellationToken);

        await characterService.RestoreWoundsFromRestAsync(domainEvent.AggregateIdentity, ev.EntityId,
            ActivityDuration.Medium, context, cancellationToken);

        var character = await characterService.GetCharacterAsync(ev.EntityId, context, cancellationToken);

        var embed = new EmbedBuilder().WithTitle($"Level {ev.NewLevel.CurrentLevel}!")
            .WithDescription($"{character.Value.Name} has reached level {ev.NewLevel.CurrentLevel}!")
            .WithColor(Color.Gold).Build();

        await channelManager.SendToGuildHallAsync(domainEvent.AggregateIdentity, string.Empty, context, embed);
    }
}