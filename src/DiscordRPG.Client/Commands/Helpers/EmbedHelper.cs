using Discord;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Commands.Helpers;

public static class EmbedHelper
{
    public static Embed GetEquipAsEmbed(Equipment? equipment)
    {
        if (equipment is null)
            return new EmbedBuilder().WithColor(Color.DarkGrey).WithDescription("Nothing equipped").Build();

        if (equipment is Weapon weapon)
            return new EmbedBuilder()
                .WithColor(Color.DarkBlue)
                .WithTitle(weapon.Name + $" (Lvl {weapon.Level})")
                .WithDescription(weapon.Description)
                .AddField("Damage", $"{weapon.DamageValue} {weapon.DamageType}")
                .AddField("Attribute", weapon.DamageAttribute.ToString())
                .AddField("Worth", weapon.Worth + "$")
                .AddField("STR", weapon.Strength, true)
                .AddField("VIT", weapon.Vitality, true)
                .AddField("\u200B", "\u200B", true)
                .AddField("AGI", weapon.Agility, true)
                .AddField("INT", weapon.Intelligence, true)
                .AddField("\u200B", "\u200B", true)
                .Build();

        return new EmbedBuilder()
            .WithColor(Color.DarkBlue)
            .WithTitle(equipment.Name + $" (Lvl {equipment.Level})")
            .WithDescription(equipment.Description)
            .AddField("Armor", equipment.Armor, true)
            .AddField("Magic Armor", equipment.MagicArmor, true)
            .AddField("Worth", equipment.Worth + "$")
            .AddField("STR", equipment.Strength, true)
            .AddField("VIT", equipment.Vitality, true)
            .AddField("\u200B", "\u200B", true)
            .AddField("AGI", equipment.Agility, true)
            .AddField("INT", equipment.Intelligence, true)
            .AddField("\u200B", "\u200B", true)
            .Build();
    }

    public static Embed GetMoneyAsEmbed(int money)
    {
        return new EmbedBuilder().AddField("Money", $"{money}$").Build();
    }
}