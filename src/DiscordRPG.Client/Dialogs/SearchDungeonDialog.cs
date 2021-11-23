namespace DiscordRPG.Client.Dialogs;

public class SearchDungeonDialog : Dialog
{
    public SearchDungeonDialog()
    {
    }

    public SearchDungeonDialog(ulong userId) : base(userId)
    {
    }

    public uint Level { get; set; }
    public string CharId { get; set; }
}