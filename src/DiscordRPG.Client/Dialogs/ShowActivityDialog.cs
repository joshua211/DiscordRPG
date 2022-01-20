using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Client.Dialogs;

public class ShowActivityDialog : Dialog
{
    public ShowActivityDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public ActivityReadModel Activity { get; set; }
    public CharacterReadModel Character { get; set; }
    public GuildId GuildId { get; set; }
}