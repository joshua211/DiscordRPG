using DiscordRPG.Core.Entities;

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
}