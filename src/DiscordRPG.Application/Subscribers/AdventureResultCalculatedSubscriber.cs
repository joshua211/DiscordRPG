using System.Text;
using Discord;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.Events;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Character.Commands;
using DiscordRPG.Domain.Services;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Subscribers;
using Weighted_Randomizer;

namespace DiscordRPG.Application.Subscribers;

public class
    AdventureResultCalculatedSubscriber : ISubscribeSynchronousTo<GuildAggregate, GuildId, AdventureResultCalculated>
{
    private readonly ICommandBus bus;
    private readonly IChannelManager channelManager;
    private readonly ICharacterService characterService;
    private readonly IDungeonService dungeonService;
    private readonly IExperienceCurve experienceCurve;
    private readonly ILogger logger;

    public AdventureResultCalculatedSubscriber(ICharacterService characterService, ICommandBus bus,
        IExperienceCurve experienceCurve, ILogger logger, IChannelManager channelManager,
        IDungeonService dungeonService)
    {
        this.characterService = characterService;
        this.bus = bus;
        this.experienceCurve = experienceCurve;
        this.channelManager = channelManager;
        this.dungeonService = dungeonService;
        this.logger = logger.WithContext(GetType());
    }

    public async Task HandleAsync(IDomainEvent<GuildAggregate, GuildId, AdventureResultCalculated> domainEvent,
        CancellationToken cancellationToken)
    {
        var id = domainEvent.Metadata["transaction-id"];
        var context = TransactionContext.With(id);
        var ev = domainEvent.AggregateEvent;

        var character = await characterService.GetCharacterAsync(ev.CharacterId, context, cancellationToken);
        var dungeon = await dungeonService.GetDungeonAsync(ev.DungeonId, context, cancellationToken);

        var newWounds = character.Value.Wounds.ToList();
        newWounds.AddRange(ev.AdventureResult.Wounds);

        if (newWounds.Sum(w => w.DamageValue) >= character.Value.CurrentHealth)
        {
            var selector = new DynamicWeightedRandomizer<bool>
            {
                {true, (int) dungeon.Value.Level.Value},
                {false, character.Value.Luck}
            };
            var hasActuallyDied = selector.NextWithReplacement();
            if (hasActuallyDied)
            {
                var embed = new EmbedBuilder().WithTitle("Exploration failed!").WithDescription(
                        $"{character.Value.Name.Value}, it seems like you were no match for the level {dungeon.Value.Level.Value} dungeon **{dungeon.Value.Name.Value}**. Sadly, you have died due to a {newWounds.Last().Description}.")
                    .Build();

                var deathCmd = new KillCharacterCommand(domainEvent.AggregateIdentity, ev.CharacterId, context);
                await bus.PublishAsync(deathCmd, cancellationToken);

                await channelManager.SendToChannelAsync(domainEvent.AggregateIdentity, new ChannelId(dungeon.Value.Id),
                    $"<@{character.Value.Id}>", context, embed);

                return;
            }
            else
            {
                var embed = new EmbedBuilder().WithTitle("Exploration failed!").WithDescription(
                        $"{character.Value.Name.Value}, it seems like you were no match for the level {dungeon.Value.Level.Value} dungeon **{dungeon.Value.Name.Value}**. You've managed to survive, but are heavily wounded and lost everything you found in this dungeon")
                    .Build();
                while (newWounds.Sum(w => w.DamageValue) >= character.Value.CurrentHealth)
                    newWounds.Remove(newWounds.Last());
                var survivedWoundsCommand =
                    new ChangeWoundsCommand(domainEvent.AggregateIdentity, ev.CharacterId, newWounds, context);
                await bus.PublishAsync(survivedWoundsCommand, cancellationToken);

                await channelManager.SendToChannelAsync(domainEvent.AggregateIdentity, new ChannelId(dungeon.Value.Id),
                    $"<@{character.Value.Id}>", context, embed);

                return;
            }
        }

        var woundsCmd = new ChangeWoundsCommand(domainEvent.AggregateIdentity, ev.CharacterId, newWounds, context);
        await bus.PublishAsync(woundsCmd, cancellationToken);
        var newItems = character.Value.Inventory.ToList();
        foreach (var item in ev.AdventureResult.Items)
        {
            var existing = newItems.FirstOrDefault(i => i == item);
            if (existing is not null)
                existing.IncreaseAmount(item.Amount);
            else
                newItems.Add(item);
        }

        var invCmd = new ChangeInventoryCommand(domainEvent.AggregateIdentity, ev.CharacterId, newItems, context);
        await bus.PublishAsync(invCmd, cancellationToken);

        var newLevel = character.Value.Level.Add(ev.AdventureResult.Experience, experienceCurve);
        var expCommand = new GainLevelCommand(domainEvent.AggregateIdentity, ev.CharacterId, newLevel, context);
        await bus.PublishAsync(expCommand, cancellationToken);

        var sb = new StringBuilder();
        sb.AppendLine(
            $"<@{character.Value.Id}> You've completed the level {dungeon.Value.Level.Value} dungeon **{dungeon.Value.Name.Value}**!");
        sb.Append($"You gained {ev.AdventureResult.Experience} exp");
        if (ev.AdventureResult.Experience >= character.Value.Level.RequiredExp)
            sb.Append($" and leveled up to level {character.Value.Level.CurrentLevel + 1}");
        sb.AppendLine("!");
        sb.AppendLine();
        if (ev.AdventureResult.Items.Any())
        {
            sb.AppendLine("In the dungeon you found the following items:");
            foreach (var item in ev.AdventureResult.Items)
            {
                if (item.Amount > 1)
                    sb.AppendLine($" - _{item} x {item.Amount}_");
                else
                    sb.AppendLine($" - _{item}_");
            }

            sb.AppendLine();
        }

        if (ev.AdventureResult.Wounds.Any())
        {
            sb.AppendLine("But you also sustained these wounds:");
            foreach (var wound in ev.AdventureResult.Wounds)
            {
                sb.AppendLine($" - _{wound}_");
            }
        }

        await channelManager.SendToChannelAsync(domainEvent.AggregateIdentity,
            new ChannelId(domainEvent.AggregateEvent.DungeonId.Value), sb.ToString(), context);
    }
}