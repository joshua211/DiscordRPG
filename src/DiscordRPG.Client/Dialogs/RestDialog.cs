using DiscordRPG.Core.Enums;

namespace DiscordRPG.Client.Dialogs;

public class RestDialog : Dialog
{
    public RestDialog(ulong userId) : base(userId)
    {
    }

    public string CharId { get; set; }
    public ActivityDuration Duration { get; set; }
}