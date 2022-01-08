using DiscordRPG.Core.Entities;

namespace DiscordRPG.Client.Dialogs;

public class ShowActivityDialog : Dialog
{
    public ShowActivityDialog(ulong userId) : base(userId)
    {
    }

    public Activity Activity { get; set; }
    public Character Character { get; set; }
}