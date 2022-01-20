using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Client.Dialogs;

public class ShowAllDungeonsDialog : Dialog
{
    public ShowAllDungeonsDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public GuildId GuildId { get; set; }
}