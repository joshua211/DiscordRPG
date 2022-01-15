using Discord;
using DiscordRPG.Common;

namespace DiscordRPG.Client.Dialogs;

public abstract class Dialog
{
    protected Dialog(ulong userId, TransactionContext context)
    {
        UserId = userId;
        Context = context;
    }


    public ulong UserId { get; private set; }
    public TransactionContext Context { get; set; }
}

public interface IShareableDialog
{
    Embed ShareableEmbed { get; set; }
}

public interface IPageableDialog
{
    int CurrentPage { get; set; }
}