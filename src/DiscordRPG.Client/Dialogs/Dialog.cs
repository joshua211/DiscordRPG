namespace DiscordRPG.Client.Dialogs;

public abstract class Dialog
{
    protected Dialog(ulong userId)
    {
        UserId = userId;
    }

    public Dialog()
    {
    }

    public ulong UserId { get; private set; }
}