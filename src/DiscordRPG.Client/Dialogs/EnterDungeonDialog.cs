namespace DiscordRPG.Client.Dialogs;

public class EnterDungeonDialog : Dialog
{
    public EnterDungeonDialog(ulong userId) : base(userId)
    {
    }

    public EnterDungeonDialog()
    {
    }

    public ulong DungeonId { get; set; }
}