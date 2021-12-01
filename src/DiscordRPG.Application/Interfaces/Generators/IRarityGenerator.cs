namespace DiscordRPG.Application.Interfaces.Generators;

public interface IRarityGenerator
{
    Rarity GenerateRarityFromActivityDuration(ActivityDuration duration);
}