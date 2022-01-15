using DiscordRPG.Common;
using DiscordRPG.Domain.Entities.Activity.Enums;

namespace DiscordRPG.Client.Dialogs;

public class RestDialog : Dialog
{
    public RestDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public string CharId { get; set; }
    public ActivityDuration Duration { get; set; }
    public ulong ServerId { get; set; }
}