using Discord;
using DiscordRPG.Domain.Aggregates.Activity.Enums;

namespace DiscordRPG.Client.Commands.Helpers;

public static class OptionHelper
{
    public static SlashCommandOptionBuilder GetActivityDurationBuilder(string description)
    {
        return new SlashCommandOptionBuilder()
            .WithName("duration")
            .WithDescription(description)
            .WithType(ApplicationCommandOptionType.Integer)
            .WithRequired(true)
            .AddChoice("quick", (int) ActivityDuration.Quick)
            .AddChoice("short", (int) ActivityDuration.Short)
            .AddChoice("medium", (int) ActivityDuration.Medium)
            .AddChoice("long", (int) ActivityDuration.Long);
    }
}