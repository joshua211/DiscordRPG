using DiscordRPG.Common;

namespace DiscordRPG.Client.Dialogs;

public class ShowAllDungeonsDialog : Dialog
{
    public ShowAllDungeonsDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }
}