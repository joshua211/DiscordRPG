namespace DiscordRPG.Common.Extensions;

public static class IntExtensions
{
    /// <summary>
    /// Rounds to the nearest lower power of ten. Returns 1 instead of zero if i is smaller than 10
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static uint RoundOff(this uint i)
    {
        var rounded = i - (i % 10);
        return rounded <= 0 ? 1 : rounded;
    }
}