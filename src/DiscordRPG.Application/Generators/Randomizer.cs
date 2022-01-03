using DiscordRPG.WeightedRandom;

namespace DiscordRPG.Application.Generators;

public class Randomizer : IRandomizer
{
    public int GetRandomized(int baseValue, float deviation)
    {
        var upper = (int) (baseValue + baseValue * deviation);
        var lower = (int) (baseValue - baseValue * deviation);
        var selector = new DynamicRandomSelector<int>();
        for (var i = lower; i < upper; i++)
        {
            selector.Add(i, 1);
        }

        return selector.Build().SelectRandomItem();
    }
}

public interface IRandomizer
{
    int GetRandomized(int baseValue, float deviation);
}