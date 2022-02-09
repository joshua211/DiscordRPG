using Discord;
using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;

namespace DiscordRPG.Client.Commands.Helpers;

public static class EmbedHelper
{
    public static Embed GetItemAsEmbed(Item? item, float worthMulti = 1, Item? comparison = null)
    {
        if (item is null)
            return new EmbedBuilder().WithColor(Color.DarkGrey).WithDescription("Nothing equipped").Build();

        var worth = (int) (item.Worth * worthMulti);

        switch (item.ItemType)
        {
            case ItemType.Equipment:
                return new EmbedBuilder()
                    .WithColor(Color.DarkBlue)
                    .WithTitle(item.Name + $" (Lvl {item.Level})")
                    .WithDescription(item.Description)
                    .AddField("Rarity", item.Rarity)
                    .AddField(item.ItemEffect?.Name ?? "No status effect",
                        item.ItemEffect is null
                            ? "-"
                            : $"{item.ItemEffect.StatusEffectType} ({item.ItemEffect.Modifier * 100}%)")
                    .AddField("Armor",
                        comparison is null
                            ? $"{item.Armor}"
                            : $"{item.Armor}({CompareValue(item.Armor, comparison.Armor)})", true)
                    .AddField("Magic Armor",
                        comparison is null
                            ? $"{item.MagicArmor}"
                            : $"{item.MagicArmor}({CompareValue(item.MagicArmor, comparison.MagicArmor)})", true)
                    .AddField("Worth", worth + "$")
                    .AddField("STR",
                        comparison is null
                            ? $"{item.Strength}"
                            : $"{item.Strength}({CompareValue(item.Strength, comparison.Strength)})", true)
                    .AddField("VIT",
                        comparison is null
                            ? $"{item.Vitality}"
                            : $"{item.Vitality}({CompareValue(item.Vitality, comparison.Vitality)})", true)
                    .AddField("\u200B", "\u200B", true)
                    .AddField("AGI",
                        comparison is null
                            ? $"{item.Agility}"
                            : $"{item.Agility}({CompareValue(item.Agility, comparison.Agility)})", true)
                    .AddField("INT",
                        comparison is null
                            ? $"{item.Intelligence}"
                            : $"{item.Intelligence}({CompareValue(item.Intelligence, comparison.Intelligence)})",
                        true)
                    .AddField("\u200B", "\u200B", true)
                    .Build();
            case ItemType.Weapon:
                return new EmbedBuilder()
                    .WithColor(Color.DarkBlue)
                    .WithTitle(item.Name + $" (Lvl {item.Level})")
                    .WithDescription(item.Description)
                    .AddField("Rarity", item.Rarity)
                    .AddField(item.ItemEffect?.Name ?? "No status effect",
                        item.ItemEffect is null
                            ? "-"
                            : $"{item.ItemEffect.StatusEffectType} ({item.ItemEffect.Modifier * 100}%)")
                    .AddField("Damage",
                        comparison is null
                            ? $"{item.DamageValue} {item.DamageType}"
                            : $"{item.DamageValue}({CompareValue(item.DamageValue, comparison.DamageValue)}) {item.DamageType}")
                    .AddField("Attribute", item.DamageAttribute.ToString())
                    .AddField("Worth", worth + "$")
                    .AddField("STR",
                        comparison is null
                            ? $"{item.Strength}"
                            : $"{item.Strength}({CompareValue(item.Strength, comparison.Strength)})", true)
                    .AddField("VIT",
                        comparison is null
                            ? $"{item.Vitality}"
                            : $"{item.Vitality}({CompareValue(item.Vitality, comparison.Vitality)})", true)
                    .AddField("\u200B", "\u200B", true)
                    .AddField("AGI",
                        comparison is null
                            ? $"{item.Agility}"
                            : $"{item.Agility}({CompareValue(item.Agility, comparison.Agility)})", true)
                    .AddField("INT",
                        comparison is null
                            ? $"{item.Intelligence}"
                            : $"{item.Intelligence}({CompareValue(item.Intelligence, comparison.Intelligence)})", true)
                    .AddField("\u200B", "\u200B", true)
                    .Build();
            case ItemType.Consumable:
            case ItemType.Item:
            default:
                return new EmbedBuilder()
                    .WithTitle(item.Name + $" (Lvl {item.Level})")
                    .WithDescription(item.Description)
                    .WithFooter("Selling items will sell your entire amount at once")
                    .WithColor(Color.DarkBlue)
                    .AddField("Rarity", item.Rarity)
                    .AddField("Worth", worth + "$")
                    .AddField("Amount", item.Amount)
                    .Build();
        }
    }

    public static Embed GetMoneyAsEmbed(int money)
    {
        var builder = new EmbedBuilder();
        builder.AddField("Money", $"{money}$");
        builder.WithFooter("You are currently selling for 70% of the item price");

        return builder.Build();
    }

    public static Embed DungeonAsEmbed(DungeonReadModel dungeon)
    {
        return new EmbedBuilder()
            .WithTitle(dungeon.Name.Value)
            .WithColor(Color.Purple)
            .AddField("Rarity", dungeon.Rarity.ToString())
            .AddField("Level", dungeon.Level.Value)
            .AddField("Explorations", $"{dungeon.Explorations.Value}")
            .WithFooter(
                "This dungeon will be deleted if no explorations are left or if it has not been used for 24 hours")
            .Build();
    }

    public static Embed RecipeAsEmbed(Recipe recipe)
    {
        var builder = new EmbedBuilder().WithTitle(recipe.Name).WithDescription(recipe.Description);
        foreach (var ingredient in recipe.Ingredients)
        {
            builder.AddField($"{ingredient.Rarity} {ingredient.Name} Lvl. {ingredient.Level}", ingredient.Amount);
        }

        return builder.Build();
    }

    private static string CompareValue(int value1, int value2)
    {
        return value1 < value2 ? $"{value1 - value2}" : $"+{Math.Abs(value2 - value1)}";
    }
}