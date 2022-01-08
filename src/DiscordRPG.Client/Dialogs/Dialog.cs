using Discord;

namespace DiscordRPG.Client.Dialogs;

public abstract class Dialog
{
    protected Dialog(ulong userId)
    {
        UserId = userId;
    }


    public ulong UserId { get; private set; }
}

public interface IShareableDialog
{
    Embed ShareableEmbed { get; set; }
}

public interface IPageableDialog
{
    int CurrentPage { get; set; }
}