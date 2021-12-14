namespace DiscordRPG.Application.Extensions;

public static class IntExtensions
{
    /// <summary>
    /// Rounds to the nearest power of ten. Returns 1 instead of zero if i is smaller than 10
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static uint RoundOff(this uint i)
    {
        var rounded = ((uint) Math.Round(i / 10.0)) * 10;
        return rounded <= 0 ? 1 : rounded;
    }
}