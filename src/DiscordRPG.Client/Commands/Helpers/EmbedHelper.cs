using Discord;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Commands.Helpers;

public static class EmbedHelper
{
    public static Embed GetItemAsEmbed(Item? item, float worthMulti = 1, Equipment comparison = null)
    {
        if (item is null)
            return new EmbedBuilder().WithColor(Color.DarkGrey).WithDescription("Nothing equipped").Build();

        var worth = (int) (item.Worth * worthMulti);

        if (item is Weapon weapon)
            return new EmbedBuilder()
                .WithColor(Color.DarkBlue)
                .WithTitle(weapon.Name + $" (Lvl {weapon.Level})")
                .WithDescription(weapon.Description)
                .AddField("Rarity", weapon.Rarity)
                .AddField("Damage",
                    comparison is null
                        ? $"{weapon.DamageValue} {weapon.DamageType}"
                        : $"{weapon.DamageValue}({CompareValue(weapon.DamageValue, ((Weapon) comparison).DamageValue)}) {weapon.DamageType}")
                .AddField("Attribute", weapon.DamageAttribute.ToString())
                .AddField("Worth", worth + "$")
                .AddField("STR",
                    comparison is null
                        ? $"{weapon.Strength}"
                        : $"{weapon.Strength}({CompareValue(weapon.Strength, comparison.Strength)})", true)
                .AddField("VIT",
                    comparison is null
                        ? $"{weapon.Vitality}"
                        : $"{weapon.Vitality}({CompareValue(weapon.Vitality, comparison.Vitality)})", true)
                .AddField("\u200B", "\u200B", true)
                .AddField("AGI",
                    comparison is null
                        ? $"{weapon.Agility}"
                        : $"{weapon.Agility}({CompareValue(weapon.Agility, comparison.Agility)})", true)
                .AddField("INT",
                    comparison is null
                        ? $"{weapon.Intelligence}"
                        : $"{weapon.Intelligence}({CompareValue(weapon.Intelligence, comparison.Intelligence)})", true)
                .AddField("\u200B", "\u200B", true)
                .Build();
        if (item is Equipment equipment)
            return new EmbedBuilder()
                .WithColor(Color.DarkBlue)
                .WithTitle(equipment.Name + $" (Lvl {equipment.Level})")
                .WithDescription(equipment.Description)
                .AddField("Rarity", equipment.Rarity)
                .AddField("Armor",
                    comparison is null
                        ? $"{equipment.Armor}"
                        : $"{equipment.Armor}({CompareValue(equipment.Armor, comparison.Strength)})", true)
                .AddField("Magic Armor",
                    comparison is null
                        ? $"{equipment.MagicArmor}"
                        : $"{equipment.MagicArmor}({CompareValue(equipment.MagicArmor, comparison.MagicArmor)})", true)
                .AddField("Worth", worth + "$")
                .AddField("STR",
                    comparison is null
                        ? $"{equipment.Strength}"
                        : $"{equipment.Strength}({CompareValue(equipment.Strength, comparison.Strength)})", true)
                .AddField("VIT",
                    comparison is null
                        ? $"{equipment.Vitality}"
                        : $"{equipment.Vitality}({CompareValue(equipment.Vitality, comparison.Vitality)})", true)
                .AddField("\u200B", "\u200B", true)
                .AddField("AGI",
                    comparison is null
                        ? $"{equipment.Agility}"
                        : $"{equipment.Agility}({CompareValue(equipment.Agility, comparison.Agility)})", true)
                .AddField("INT",
                    comparison is null
                        ? $"{equipment.Intelligence}"
                        : $"{equipment.Intelligence}({CompareValue(equipment.Intelligence, comparison.Intelligence)})",
                    true)
                .AddField("\u200B", "\u200B", true)
                .Build();

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

    public static Embed GetMoneyAsEmbed(int money)
    {
        var builder = new EmbedBuilder();
        builder.AddField("Money", $"{money}$");
        builder.WithFooter("You are currently selling for 70% of the item price");

        return builder.Build();
    }

    public static Embed DungeonAsEmbed(Dungeon dungeon)
    {
        return new EmbedBuilder()
            .WithTitle(dungeon.Name)
            .WithColor(Color.Purple)
            .AddField("Rarity", dungeon.Rarity.ToString())
            .AddField("Level", dungeon.DungeonLevel)
            .AddField("Explorations", $"{dungeon.ExplorationsLeft}")
            .WithFooter(
                "This dungeon will be deleted if no explorations are left or if it has not been used for 24 hours")
            .Build();
    }

    private static string CompareValue(int value1, int value2)
    {
        return value1 > value2 ? $"+{value1 - value2}" : $"{value1 - value2}";
    }
}