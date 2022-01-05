namespace DiscordRPG.Application.Generators;

public class Randomizer : IRandomizer
{
    private static Random random = new Random();

    public int GetRandomized(int baseValue, float deviation)
    {
        var upper = (int) (baseValue + baseValue * deviation);
        var lower = (int) (baseValue - baseValue * deviation);

        return upper == lower ? baseValue : random.Next(lower, upper + 1);
    }
}

public interface IRandomizer
{
    int GetRandomized(int baseValue, float deviation);
}