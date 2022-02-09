using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Activity.Enums;

namespace DiscordRPG.Client.Dialogs;

public class RestDialog : Dialog
{
    public RestDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public string CharId { get; set; }
    public ActivityDuration Duration { get; set; }
    public string GuildId { get; set; }
}