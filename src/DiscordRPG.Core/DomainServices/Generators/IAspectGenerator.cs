namespace DiscordRPG.Core.DomainServices.Generators;

public interface IAspectGenerator
{
    Aspect GetRandomAspect(Rarity rarity);
}