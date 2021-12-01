namespace DiscordRPG.Application.Generators;

public abstract class GeneratorBase
{
    protected static readonly Random random;

    static GeneratorBase()
    {
        random = new Random();
    }
}