using DiscordRPG.Application.Data.Models;

namespace DiscordRPG.Application.Data;

public static class Recipes
{
    public static Recipe HealthPotion1 => new Recipe(0, 0, 10, 10, Rarity.Common, 1, "HealthPotion1");
}