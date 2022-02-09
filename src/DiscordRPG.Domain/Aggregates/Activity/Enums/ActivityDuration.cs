namespace DiscordRPG.Domain.Aggregates.Activity.Enums;

/// <summary>
/// Activity Duration in minutes
/// </summary>
public enum ActivityDuration
{
    /// <summary>
    /// 10 Minutes
    /// 5% Health
    /// </summary>
    Quick = 10,

    /// <summary>
    /// 1 Hour
    /// 25% Health
    /// </summary>
    Short = 60,

    /// <summary>
    /// 3 Hours
    /// 50% health
    /// </summary>
    Medium = 180,

    /// <summary>
    /// 8 Hours
    /// 75% health
    /// </summary>
    Long = 480,

    /// <summary>
    /// 12 Hours
    /// 100% health
    /// </summary>
    ExtraLong = 720
}