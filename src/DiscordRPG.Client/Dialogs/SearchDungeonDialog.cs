using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;

namespace DiscordRPG.Client.Dialogs;

public class SearchDungeonDialog : Dialog
{
    public SearchDungeonDialog()
    {
    }

    public SearchDungeonDialog(ulong userId) : base(userId)
    {
    }

    public Character Character { get; set; }
    public ulong ServerId { get; set; }
    public ActivityDuration Duration { get; set; }
}