using Discord;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Commands.Helpers;

public static class EmbedHelper
{
    public static Embed GetItemAsEmbed(Item? item, float worthMulti = 1)
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
                .AddField("Damage", $"{weapon.DamageValue} {weapon.DamageType}")
                .AddField("Attribute", weapon.DamageAttribute.ToString())
                .AddField("Worth", worth + "$")
                .AddField("STR", weapon.Strength, true)
                .AddField("VIT", weapon.Vitality, true)
                .AddField("\u200B", "\u200B", true)
                .AddField("AGI", weapon.Agility, true)
                .AddField("INT", weapon.Intelligence, true)
                .AddField("\u200B", "\u200B", true)
                .Build();
        if (item is Equipment equipment)
            return new EmbedBuilder()
                .WithColor(Color.DarkBlue)
                .WithTitle(equipment.Name + $" (Lvl {equipment.Level})")
                .WithDescription(equipment.Description)
                .AddField("Rarity", equipment.Rarity)
                .AddField("Armor", equipment.Armor, true)
                .AddField("Magic Armor", equipment.MagicArmor, true)
                .AddField("Worth", worth + "$")
                .AddField("STR", equipment.Strength, true)
                .AddField("VIT", equipment.Vitality, true)
                .AddField("\u200B", "\u200B", true)
                .AddField("AGI", equipment.Agility, true)
                .AddField("INT", equipment.Intelligence, true)
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
}