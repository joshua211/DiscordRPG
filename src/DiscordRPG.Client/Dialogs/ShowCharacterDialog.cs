using Discord;
using DiscordRPG.Common;

namespace DiscordRPG.Client.Dialogs;

public class ShowCharacterDialog : Dialog, IShareableDialog
{
    public ShowCharacterDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public Embed ShareableEmbed { get; set; }
}