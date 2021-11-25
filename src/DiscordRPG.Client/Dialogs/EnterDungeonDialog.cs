using DiscordRPG.Core.Entities;

namespace DiscordRPG.Client.Dialogs;

public class EnterDungeonDialog : Dialog
{
    public EnterDungeonDialog(ulong userId) : base(userId)
    {
    }

    public EnterDungeonDialog()
    {
    }

    public Dungeon Dungeon { get; set; }
    public string CharId { get; set; }
}